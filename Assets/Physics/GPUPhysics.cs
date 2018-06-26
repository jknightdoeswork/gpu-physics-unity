using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GPUPhysics : MonoBehaviour {

	public bool m_debugWireframe;
	private bool m_lastDebugWireframe;
	// set from editor
	public ComputeShader m_computeShader;
	public Mesh cubeMesh {
		get {
			return m_debugWireframe ? CjLib.PrimitiveMeshFactory.BoxWireframe() : CjLib.PrimitiveMeshFactory.BoxFlatShaded();
		}
	}
	public Mesh sphereMesh {
		get {
			return CjLib.PrimitiveMeshFactory.SphereWireframe(6,6);
		}
	}
	public Mesh lineMesh {
		get {
			return CjLib.PrimitiveMeshFactory.Line(Vector3.zero, new Vector3(1.0f,1.0f,1.0f));
		}
	}
	public Material cubeMaterial;
	public Material sphereMaterial;
	public Material lineMaterial;
	public Bounds m_bounds;
	public float m_cubeMass;
	public float scale;
	public float springCoefficient;
	public float dampingCoefficient;
	public float tangentialCoefficient;
	public float gravityCoefficient;
	public float frictionCoefficient;
	public float angularFrictionCoefficient;
	public float angularForceScalar;
	public int x = 10;
	public int y = 20;
	public int z = 5;
	public int debug_particle_id_count;
	public int gridX;
	public int gridY;
	public int gridZ;
	public Vector3 m_firstCubeLocation; // eg
	public Vector3 m_firstCubeVelocity;
	public Vector3 m_firstCubeRotation;
	// debug
	public bool m_debugRender;
	
	// calculated
	private int total;
	private Vector3 m_cubeScale;
	private Matrix4x4[] m_matrices;

	// data
	private ComputeBuffer m_rigidBodyPositions;					// float3
	private ComputeBuffer m_rigidBodyQuaternions;				// float4
	private ComputeBuffer m_rigidBodyAngularVelocities;			// float3
	private ComputeBuffer m_rigidBodyVelocities;				// float3

	private ComputeBuffer m_particleInitialRelativePositions; 	// float3
	private ComputeBuffer m_particlePositions;					// float3
	private ComputeBuffer m_particleRelativePositions;			// float3
	private ComputeBuffer m_particleVelocities;					// float3
	private ComputeBuffer m_particleForces;						// float3


	private ComputeBuffer m_debugParticleIds;					// int
	private ComputeBuffer m_debugParticleVoxelPositions;		// int3 // the per particle grid locations
	private ComputeBuffer m_voxelCollisionGrid;					// int4

	private CommandBuffer m_commandBuffer;

	public Vector3[] 		positionArray;							// cpu->matrix
	public Quaternion[] 	quaternionArray;						// cpu->matrix
	public Vector3[]		particleForcesArray;
	public Vector3[]		particleVelocities;
	public Vector3[]		rigidBodyVelocitiesArray;
	public Vector3[]		particlePositions;
	public Vector3[]		particleRelativePositions;
	public Vector3[] 		particleInitialRelativePositions;

	public int[]			voxelGridArray;
	public int[]			particleVoxelPositionsArray;
	public int[]			debugParticleIds;
	private int m_kernel_generateParticleValues;
	private int m_kernel_clearGrid;
	private int m_kernel_populateGrid;
	private int m_kernel_collisionDetection;
	private int m_kernel_computeMomenta;
	private int m_kernel_computePositionAndRotation;


	private int m_threadGroupsPerRigidBody;
	private int m_threadGroupsPerParticle;
	private int m_threadGroupsPerGridCell;
	private int m_deltaTimeShaderProperty;

	public Vector3 gridStartPosition;

	private ComputeBuffer m_bufferWithArgs;
	private ComputeBuffer m_bufferWithSphereArgs;
	private ComputeBuffer m_bufferWithLineArgs;

	void Start () {
		Application.targetFrameRate = 300;
		// Create initial positions
		total 			= x*y*z;
		m_matrices 		= new Matrix4x4[total];
		positionArray 	= new Vector3[total];
		quaternionArray	= new Quaternion[total];
		rigidBodyVelocitiesArray = new Vector3[total];
		m_cubeScale 	= new Vector3(scale,scale,scale);
		m_deltaTimeShaderProperty = Shader.PropertyToID("deltaTime");

		const int particlesPerBody			= 8;
		int n_particles						= particlesPerBody * total;
		int numGridCells					= gridX*gridY*gridZ;

		particleForcesArray = new Vector3[n_particles];

		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				for (int k = 0; k < z; k++) {
					positionArray[IDX(i,j,k)] = new Vector3(i*scale, j*scale, k*scale) + new Vector3(0.5f * scale, 0.5f * scale, 0.5f * scale);
					quaternionArray[IDX(i,j,k)] = Quaternion.identity;
					rigidBodyVelocitiesArray[IDX(i,j,k)] = Vector3.zero;
				}
			}
		}
		// rigid body velocities
		positionArray[IDX(0,y-1,0)] = m_firstCubeLocation;
		rigidBodyVelocitiesArray[IDX(0,y-1,0)] = m_firstCubeVelocity;
		quaternionArray[IDX(0, y - 1, 0)] = Quaternion.Euler(m_firstCubeRotation);
		particleVelocities = new Vector3[n_particles];
		particlePositions = new Vector3[n_particles];
		particleRelativePositions = new Vector3[n_particles];
		debugParticleIds = new int[debug_particle_id_count];
		voxelGridArray = new int[numGridCells * 4];
		particleVoxelPositionsArray = new int[n_particles * 3];

		// Get Kernels
		m_kernel_generateParticleValues 		= m_computeShader.FindKernel("GenerateParticleValues");
		m_kernel_clearGrid 						= m_computeShader.FindKernel("ClearGrid");
		m_kernel_populateGrid					= m_computeShader.FindKernel("PopulateGrid");
		m_kernel_collisionDetection				= m_computeShader.FindKernel("CollisionDetection");
		m_kernel_computeMomenta					= m_computeShader.FindKernel("ComputeMomenta");
		m_kernel_computePositionAndRotation		= m_computeShader.FindKernel("ComputePositionAndRotation");

		// Count Thread Groups
		m_threadGroupsPerRigidBody				= Mathf.CeilToInt(total / 8.0f);
		m_threadGroupsPerParticle				= Mathf.CeilToInt(n_particles / 8f);
		m_threadGroupsPerGridCell				= Mathf.CeilToInt((gridX*gridY*gridZ) / 8f);

		// Create initial buffers
		const int floatThree 				= 3 * sizeof(float);
		const int floatFour 				= 4 * sizeof(float);
		const int intFour 					= 4 * sizeof(int);
		const int intThree 					= 3 * sizeof(int);

		m_rigidBodyPositions				= new ComputeBuffer(total, floatThree);
		m_rigidBodyQuaternions				= new ComputeBuffer(total, floatFour);
		m_rigidBodyAngularVelocities 		= new ComputeBuffer(total, floatThree);
		m_rigidBodyVelocities 				= new ComputeBuffer(total, floatThree);

		m_particleInitialRelativePositions	= new ComputeBuffer(n_particles, floatThree);
		m_particlePositions					= new ComputeBuffer(n_particles, floatThree);
		m_particleRelativePositions			= new ComputeBuffer(n_particles, floatThree);
		m_particleVelocities				= new ComputeBuffer(n_particles, floatThree);
		m_particleForces					= new ComputeBuffer(n_particles, floatThree);

		m_voxelCollisionGrid				= new ComputeBuffer(numGridCells, intFour);
		m_debugParticleVoxelPositions		= new ComputeBuffer(n_particles, intThree);
		m_debugParticleIds					= new ComputeBuffer(debug_particle_id_count, sizeof(int));
		Debug.Log("nparticles: " + n_particles);
		// initialize constants
		int[] gridDimensions = new int[]{gridX, gridY, gridZ};
		m_computeShader.SetInts("gridDimensions", gridDimensions);
		m_computeShader.SetInt("gridMax", numGridCells);
		m_computeShader.SetFloat("particleDiameter", scale*0.5f);
		m_computeShader.SetFloat("springCoefficient", springCoefficient);
		m_computeShader.SetFloat("dampingCoefficient", dampingCoefficient);
		m_computeShader.SetFloat("frictionCoefficient", frictionCoefficient);
		m_computeShader.SetFloat("angularFrictionCoefficient", angularFrictionCoefficient);
		m_computeShader.SetFloat("gravityCoefficient", gravityCoefficient);
		m_computeShader.SetFloat("tangentialCoefficient", tangentialCoefficient);
		m_computeShader.SetFloat("angularForceScalar", angularForceScalar);
		m_computeShader.SetFloats("gridStartPosition", new float[]{gridStartPosition.x, gridStartPosition.y, gridStartPosition.z});
		m_computeShader.SetFloat("particleMass", m_cubeMass/particlesPerBody);

		// Inertial tensor of a cube formula taken from textbook:
		// "Essential Mathematics for Games and Interactive Applications"
		// by James Van Verth and Lars Bishop
		float twoDimSq = 2.0f * (scale*scale);
		float inertialTensorFactor = m_cubeMass * 1.0f/12.0f * twoDimSq;
		float[] inertialTensor = {
			inertialTensorFactor, 0.0f,	0.0f,
			0.0f, inertialTensorFactor, 0.0f,
			0.0f, 0.0f, inertialTensorFactor
		};
		m_computeShader.SetFloats("inertialTensor", inertialTensor);

		// initialize buffers
		// initial relative positions
		// super dependent on 8/rigid body
		float quarterScale = scale * 0.25f;
		particleInitialRelativePositions = new Vector3[n_particles];
		Vector3[] particleInitialsSmall = new Vector3[particlesPerBody];
		particleInitialsSmall[0] = new Vector3(quarterScale, 	quarterScale, 	quarterScale);
		particleInitialsSmall[1] = new Vector3(-quarterScale, 	quarterScale, 	quarterScale);
		particleInitialsSmall[2] = new Vector3(quarterScale, 	quarterScale, 	-quarterScale);
		particleInitialsSmall[3] = new Vector3(-quarterScale, 	quarterScale, 	-quarterScale);
		particleInitialsSmall[4] = new Vector3(quarterScale, 	-quarterScale, quarterScale);
		particleInitialsSmall[5] = new Vector3(-quarterScale, 	-quarterScale, quarterScale);
		particleInitialsSmall[6] = new Vector3(quarterScale, 	-quarterScale, -quarterScale);
		particleInitialsSmall[7] = new Vector3(-quarterScale, 	-quarterScale, -quarterScale);

		for (int i = 0; i < particleInitialRelativePositions.Length; i++) {
			particleInitialRelativePositions[i] = particleInitialsSmall[i%8];
		}

		m_particleInitialRelativePositions.SetData(particleInitialRelativePositions);

		// rigid body positions
		m_rigidBodyPositions.SetData(positionArray);

		// rigid body quaternions
		m_rigidBodyQuaternions.SetData(quaternionArray);

		m_rigidBodyVelocities.SetData(rigidBodyVelocitiesArray);

		// Set matricies to initial positions
		SetMatrices(positionArray, quaternionArray);

		// Bind buffers

		// kernel 0 GenerateParticleValues
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "rigidBodyPositions", m_rigidBodyPositions);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "rigidBodyQuaternions", m_rigidBodyQuaternions);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "rigidBodyAngularVelocities", m_rigidBodyAngularVelocities);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "rigidBodyVelocities", m_rigidBodyVelocities);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "particleInitialRelativePositions", m_particleInitialRelativePositions);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "particlePositions", m_particlePositions);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "particleRelativePositions", m_particleRelativePositions);
		m_computeShader.SetBuffer(m_kernel_generateParticleValues, "particleVelocities", m_particleVelocities);
		//m_computeShader.SetBuffer(m_kernel_generateParticleValues, "debugParticleIds", m_debugParticleIds);

		// kernel 1 ClearGrid
		m_computeShader.SetBuffer(m_kernel_clearGrid, "voxelCollisionGrid", m_voxelCollisionGrid);

		// kernel 2 Populate Grid
		m_computeShader.SetBuffer(m_kernel_populateGrid, "debugParticleVoxelPositions", m_debugParticleVoxelPositions);
		m_computeShader.SetBuffer(m_kernel_populateGrid, "voxelCollisionGrid", m_voxelCollisionGrid);
		m_computeShader.SetBuffer(m_kernel_populateGrid, "particlePositions", m_particlePositions);
		//m_computeShader.SetBuffer(m_kernel_populateGrid, "debugParticleIds", m_debugParticleIds);

		// kernel 3 Collision Detection
		m_computeShader.SetBuffer(m_kernel_collisionDetection, "particlePositions", m_particlePositions);
		m_computeShader.SetBuffer(m_kernel_collisionDetection, "particleVelocities", m_particleVelocities);
		m_computeShader.SetBuffer(m_kernel_collisionDetection, "voxelCollisionGrid", m_voxelCollisionGrid);
		m_computeShader.SetBuffer(m_kernel_collisionDetection, "particleForces", m_particleForces);

		// kernel 4 Computation of Momenta
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "particleForces", m_particleForces);
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "particleRelativePositions", m_particleRelativePositions);
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "rigidBodyAngularVelocities", m_rigidBodyAngularVelocities);
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "rigidBodyVelocities", m_rigidBodyVelocities);
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "debugParticleIds", m_debugParticleIds);

		// kernel 5 Compute Position and Rotation
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyVelocities", m_rigidBodyVelocities);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyAngularVelocities", m_rigidBodyAngularVelocities);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyPositions", m_rigidBodyPositions);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyQuaternions", m_rigidBodyQuaternions);

		// Setup Indirect Renderer
		uint[] sphereArgs = new uint[]{sphereMesh.GetIndexCount(0), (uint)n_particles, 0, 0, 0};
		m_bufferWithSphereArgs = new ComputeBuffer(1, sphereArgs.Length*sizeof(uint), ComputeBufferType.IndirectArguments);
		m_bufferWithSphereArgs.SetData(sphereArgs);

		uint[] lineArgs = new uint[]{lineMesh.GetIndexCount(0), (uint)n_particles, 0, 0, 0};
		m_bufferWithLineArgs = new ComputeBuffer(1, lineArgs.Length*sizeof(uint), ComputeBufferType.IndirectArguments);
		m_bufferWithLineArgs.SetData(lineArgs);

		cubeMaterial.SetBuffer("positions", m_rigidBodyPositions);
		cubeMaterial.SetBuffer("quaternions", m_rigidBodyQuaternions);
		sphereMaterial.SetBuffer("positions",m_particlePositions);
		sphereMaterial.SetVector("scale", new Vector4(0.25f, 0.25f, 0.25f, 1.0f));
		lineMaterial.SetBuffer("positions", m_particlePositions);
		lineMaterial.SetBuffer("vectors", m_particleVelocities);

		// Setup Command Buffer
		m_commandBuffer = new CommandBuffer();
		m_commandBuffer.BeginSample("GenerateParticleValues");
		m_commandBuffer.DispatchCompute(m_computeShader,m_kernel_generateParticleValues, m_threadGroupsPerRigidBody, 1, 1);
		m_commandBuffer.EndSample("GenerateParticleValues");

		m_commandBuffer.BeginSample("ClearGrid");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_clearGrid, m_threadGroupsPerGridCell, 1, 1);
		m_commandBuffer.EndSample("ClearGrid");

		m_commandBuffer.BeginSample("PopulateGrid");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_populateGrid, m_threadGroupsPerParticle, 1, 1);
		m_commandBuffer.EndSample("PopulateGrid");

		m_commandBuffer.BeginSample("CollisionDetection");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_collisionDetection, m_threadGroupsPerParticle, 1, 1);
		m_commandBuffer.EndSample("CollisionDetection");

		m_commandBuffer.BeginSample("ComputeMomenta");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_computeMomenta, m_threadGroupsPerRigidBody, 1 ,1);
		m_commandBuffer.EndSample("ComputeMomenta");

		m_commandBuffer.BeginSample("ComputePositions");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_computePositionAndRotation, m_threadGroupsPerRigidBody, 1, 1);
		m_commandBuffer.EndSample("ComputePositions");

		// rendering in command buffer - doesnt work for now seems like a unity bug
		#if !(UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) // Command Buffer DrawMeshInstancedIndirect doesnt work on my mac
		// rendering from command buffer via update isnt working so disabling this is necessary for delta time use
//		m_commandBuffer.BeginSample("DrawMeshInstancedIndirect");
//		m_commandBuffer.DrawMeshInstancedIndirect(cubeMesh, 0, cubeMaterial, 0, m_bufferWithArgs);
//		m_commandBuffer.EndSample("DrawMeshInstancedIndirect");
		#endif

//		Camera.main.AddCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
	}

	private void SetMatrices(Vector3[] positions, Quaternion[] quaternions) {
		for (int i = 0; i < positions.Length; i++) {
			m_matrices[i] = Matrix4x4.TRS(positions[i], quaternions[i], m_cubeScale);
		}
	}

	private int IDX(int i, int j, int k) {
		return i + (j * x) + (k * x * y);
	}

	void Update () {
		if (m_bufferWithArgs == null || m_debugWireframe != m_lastDebugWireframe) {
			uint indexCountPerInstance 		= cubeMesh.GetIndexCount(0);
			uint instanceCount 				= (uint)total;
			uint startIndexLocation 		= 0;
			uint baseVertexLocation 		= 0;
			uint startInstanceLocation 		= 0;
			uint[] args = new uint[]{indexCountPerInstance, instanceCount , startIndexLocation, baseVertexLocation, startInstanceLocation};
			m_bufferWithArgs = new ComputeBuffer(1, args.Length*sizeof(uint), ComputeBufferType.IndirectArguments);
			m_bufferWithArgs.SetData(args);
			m_lastDebugWireframe = m_debugWireframe;
		}
		/*
		//quaternionArray[IDX(0, y - 1, 0)] = Quaternion.Euler(m_firstCubeRotation);
		//m_rigidBodyQuaternions.SetData(quaternionArray);
		//positionArray[IDX(0, y -1, 0)] = m_firstCubeLocation;
		//m_rigidBodyPositions.SetData(positionArray);
		//positionArray[IDX(0,y-1,0)] = m_firstCubeLocation;
		int idx = IDX(0,y-1,0);
		m_rigidBodyQuaternions.GetData(quaternionArray);
		m_rigidBodyPositions.GetData(positionArray);
		m_particlePositions.GetData(particlePositions);
		*/
		m_computeShader.SetFloat(m_deltaTimeShaderProperty, Time.deltaTime);
		Graphics.ExecuteCommandBuffer(m_commandBuffer);
//		#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) // Command Buffer DrawMeshInstancedIndirect doesnt work on my mac
			Graphics.DrawMeshInstancedIndirect(cubeMesh, 0, cubeMaterial, m_bounds, m_bufferWithArgs);
			if (m_debugWireframe) {
				//Graphics.DrawMeshInstancedIndirect(sphereMesh, 0, sphereMaterial, m_bounds, m_bufferWithSphereArgs);
				Graphics.DrawMeshInstancedIndirect(lineMesh, 0, lineMaterial, m_bounds, m_bufferWithLineArgs);
			}
//		#endif
		/*
		m_computeShader.Dispatch(m_kernel_generateParticleValues, m_threadGroupsPerRigidBody, 1, 1);
		m_computeShader.Dispatch(m_kernel_clearGrid, m_threadGroupsPerGridCell, 1, 1);
		m_computeShader.Dispatch(m_kernel_populateGrid, m_threadGroupsPerParticle,1, 1);
		m_computeShader.Dispatch(m_kernel_collisionDetection, m_threadGroupsPerParticle, 1, 1);
		m_computeShader.Dispatch(m_kernel_computeMomenta, m_threadGroupsPerRigidBody, 1, 1);
		m_computeShader.Dispatch(m_kernel_computePositionAndRotation, m_threadGroupsPerRigidBody, 1, 1);
		*/
//			m_particleRelativePositions.GetData(particleRelativePositions);
//			m_particleVelocities.GetData(particleVelocities);
//			m_rigidBodyVelocities.GetData(rigidBodyVelocitiesArray);
//			m_particleForces.GetData(particleForcesArray);
			
//			m_particlePositions.GetData(particlePositions);
//			m_voxelCollisionGrid.GetData(voxelGridArray);
//			m_debugParticleVoxelPositions.GetData(particleVoxelPositionsArray);

			//m_rigidBodyPositions.GetData(positionArray);
			//m_rigidBodyQuaternions.GetData(quaternionArray);

//			m_debugParticleIds.GetData(debugParticleIds);
			//SetMatrices(positionArray, quaternionArray);


		//Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, m_matrices, total, null, UnityEngine.Rendering.ShadowCastingMode.On, true, 0, Camera.main);
		//Graphics.ExecuteCommandBuffer(m_commandBuffer);
		
	}

	void OnDestroy() {
		m_rigidBodyPositions.Release();
		m_rigidBodyQuaternions.Release();
		m_rigidBodyAngularVelocities.Release();
		m_rigidBodyVelocities.Release();

		m_particleInitialRelativePositions.Release();
		m_particlePositions.Release();
		m_particleRelativePositions.Release();
		m_particleVelocities.Release();
		m_particleForces.Release();

		m_debugParticleVoxelPositions.Release();
		m_voxelCollisionGrid.Release();
	}
}
