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
			return CjLib.PrimitiveMeshFactory.SphereWireframe(6, 6);
		}
	}
	public Mesh lineMesh {
		get {
			return CjLib.PrimitiveMeshFactory.Line(Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f));
		}
	}
	public Rigidbody[] comparisonCubes;
	public Material cubeMaterial;
	public Material sphereMaterial;
	public Material lineMaterial;
	public Material lineAngularMaterial;
	public Bounds m_bounds;
	public float m_cubeMass;
	public float scale;
	public int particlesPerEdge;
	public float springCoefficient;
	public float dampingCoefficient;
	public float tangentialCoefficient;
	public float gravityCoefficient;
	public float frictionCoefficient;
	public float angularFrictionCoefficient;
	public float angularForceScalar;
	public float linearForceScalar;
	public int x = 10;
	public int y = 20;
	public int z = 5;
	public int debug_particle_id_count;
	public int gridX;
	public int gridY;
	public int gridZ;
	public float dt;
	public float tick_rate;
	private float ticker;

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
	private ComputeBuffer m_rigidBodyPositions;                 // float3
	private ComputeBuffer m_previousRigidBodyPositions;			// float3
	private ComputeBuffer m_rigidBodyQuaternions;               // float4
	private ComputeBuffer m_previousRigidBodyQuaternions;		// float4
	private ComputeBuffer m_rigidBodyAngularVelocities;         // float3
	private ComputeBuffer m_rigidBodyVelocities;                // float3
	private ComputeBuffer m_rigidBodyInertialTensors;			// Matrix4x4
	private ComputeBuffer m_particleInitialRelativePositions;   // float3
	private ComputeBuffer m_particlePositions;                  // float3
	private ComputeBuffer m_particleRelativePositions;          // float3
	private ComputeBuffer m_particleVelocities;                 // float3
	private ComputeBuffer m_particleForces;                     // float3
	


	private ComputeBuffer m_debugParticleIds;                   // int
	private ComputeBuffer m_debugParticleVoxelPositions;        // int3 // the per particle grid locations
	private ComputeBuffer m_voxelCollisionGrid;                 // int4

	private CommandBuffer m_commandBuffer;


	public Vector3[] positionArray;                         // cpu->matrix
	public Quaternion[] quaternionArray;                        // cpu->matrix
	public Vector3[] particleForcesArray;
	public Vector3[] particleVelocities;
	public Vector3[] rigidBodyVelocitiesArray;
	public Vector3[] particlePositions;
	public Vector3[] particleRelativePositions;
	public Vector3[] particleInitialRelativePositions;
	public float[] rigidBodyInertialTensors;

	public int[] voxelGridArray;
	public int[] particleVoxelPositionsArray;
	public int[] debugParticleIds;
	private int m_kernel_generateParticleValues;
	private int m_kernel_clearGrid;
	private int m_kernel_populateGrid;
	private int m_kernel_collisionDetection;
	private int m_kernel_computeMomenta;
	private int m_kernel_computePositionAndRotation;
	private int m_kernelSavePreviousPositionAndRotation;

	private int m_threadGroupsPerRigidBody;
	private int m_threadGroupsPerParticle;
	private int m_threadGroupsPerGridCell;
	private int m_deltaTimeShaderProperty;

	public Quaternion m_rigidBodyQuaternion;
	public Quaternion m_comparisonQuaternion;
	public Vector3 gridStartPosition;

	public float m_maximumVelocity;
	private ComputeBuffer m_bufferWithArgs;
	private ComputeBuffer m_bufferWithSphereArgs;
	private ComputeBuffer m_bufferWithLineArgs;
	private int frameCounter;
	void Start() {
		Application.targetFrameRate = 300;
		// Create initial positions
		total = x * y * z;
		m_matrices = new Matrix4x4[total];
		positionArray = new Vector3[total];
		quaternionArray = new Quaternion[total];
		rigidBodyVelocitiesArray = new Vector3[total];
		m_cubeScale = new Vector3(scale, scale, scale);
		m_deltaTimeShaderProperty = Shader.PropertyToID("deltaTime");
		rigidBodyInertialTensors = new float[total*9];
		
		int particlesPerEdgeMinusOne = particlesPerEdge-2;
		int particlesPerBody = particlesPerEdge * particlesPerEdge * particlesPerEdge - particlesPerEdgeMinusOne*particlesPerEdgeMinusOne*particlesPerEdgeMinusOne;
		int n_particles = particlesPerBody * total;
		int numGridCells = gridX * gridY * gridZ;
		float particleDiameter = scale / particlesPerEdge;
		particleForcesArray = new Vector3[n_particles];

		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				for (int k = 0; k < z; k++) {
					positionArray[IDX(i, j, k)] = new Vector3(i * scale, j * scale, k * scale) + new Vector3(0.5f * scale, 0.5f * scale, 0.5f * scale);
					quaternionArray[IDX(i, j, k)] = Quaternion.identity;
					rigidBodyVelocitiesArray[IDX(i, j, k)] = Vector3.zero;
				}
			}
		}
		// rigid body velocities
		positionArray[IDX(0, y - 1, 0)] = m_firstCubeLocation;
		rigidBodyVelocitiesArray[IDX(0, y - 1, 0)] = m_firstCubeVelocity;
		quaternionArray[IDX(0, y - 1, 0)] = Quaternion.Euler(m_firstCubeRotation);
		particleVelocities = new Vector3[n_particles];
		particlePositions = new Vector3[n_particles];
		particleRelativePositions = new Vector3[n_particles];
		debugParticleIds = new int[debug_particle_id_count];
		voxelGridArray = new int[numGridCells * 4];
		particleVoxelPositionsArray = new int[n_particles * 3];

		// Get Kernels
		m_kernel_generateParticleValues = m_computeShader.FindKernel("GenerateParticleValues");
		m_kernel_clearGrid = m_computeShader.FindKernel("ClearGrid");
		m_kernel_populateGrid = m_computeShader.FindKernel("PopulateGrid");
		m_kernel_collisionDetection = m_computeShader.FindKernel("CollisionDetection");
		m_kernel_computeMomenta = m_computeShader.FindKernel("ComputeMomenta");
		m_kernel_computePositionAndRotation = m_computeShader.FindKernel("ComputePositionAndRotation");
		m_kernelSavePreviousPositionAndRotation = m_computeShader.FindKernel("SavePreviousPositionAndRotation");
		// Count Thread Groups
		m_threadGroupsPerRigidBody = Mathf.CeilToInt(total / 8.0f);
		m_threadGroupsPerParticle = Mathf.CeilToInt(n_particles / 8f);
		m_threadGroupsPerGridCell = Mathf.CeilToInt((gridX * gridY * gridZ) / 8f);

		// Create initial buffers
		const int floatThree = 3 * sizeof(float);
		const int floatFour = 4 * sizeof(float);
		const int intFour = 4 * sizeof(int);
		const int intThree = 3 * sizeof(int);
		const int floatNine = 9*sizeof(float);

		m_rigidBodyPositions = new ComputeBuffer(total, floatThree);
		m_previousRigidBodyPositions = new ComputeBuffer(total, floatThree);
		m_rigidBodyQuaternions = new ComputeBuffer(total, floatFour);
		m_previousRigidBodyQuaternions = new ComputeBuffer(total, floatFour);
		m_rigidBodyAngularVelocities = new ComputeBuffer(total, floatThree);
		m_rigidBodyVelocities = new ComputeBuffer(total, floatThree);
		m_rigidBodyInertialTensors = new ComputeBuffer(total, floatNine);

		m_particleInitialRelativePositions = new ComputeBuffer(n_particles, floatThree);
		m_particlePositions = new ComputeBuffer(n_particles, floatThree);
		m_particleRelativePositions = new ComputeBuffer(n_particles, floatThree);
		m_particleVelocities = new ComputeBuffer(n_particles, floatThree);
		m_particleForces = new ComputeBuffer(n_particles, floatThree);

		m_voxelCollisionGrid = new ComputeBuffer(numGridCells, intFour);
		m_debugParticleVoxelPositions = new ComputeBuffer(n_particles, intThree);
		m_debugParticleIds = new ComputeBuffer(debug_particle_id_count, sizeof(int));
		Debug.Log("nparticles: " + n_particles);
		// initialize constants
		int[] gridDimensions = new int[] { gridX, gridY, gridZ };
		m_computeShader.SetInts("gridDimensions", gridDimensions);
		m_computeShader.SetInt("gridMax", numGridCells);
		m_computeShader.SetInt("particlesPerRigidBody", particlesPerBody);
		m_computeShader.SetFloat("particleDiameter", particleDiameter);
		m_computeShader.SetFloat("springCoefficient", springCoefficient);
		m_computeShader.SetFloat("dampingCoefficient", dampingCoefficient);
		m_computeShader.SetFloat("frictionCoefficient", frictionCoefficient);
		m_computeShader.SetFloat("angularFrictionCoefficient", angularFrictionCoefficient);
		m_computeShader.SetFloat("gravityCoefficient", gravityCoefficient);
		m_computeShader.SetFloat("tangentialCoefficient", tangentialCoefficient);
		m_computeShader.SetFloat("angularForceScalar", angularForceScalar);
		m_computeShader.SetFloat("linearForceScalar", linearForceScalar);
		m_computeShader.SetFloat("particleMass", m_cubeMass / particlesPerBody);
		m_computeShader.SetFloats("gridStartPosition", new float[] { gridStartPosition.x, gridStartPosition.y, gridStartPosition.z });


		// Inertial tensor of a cube formula taken from textbook:
		// "Essential Mathematics for Games and Interactive Applications"
		// by James Van Verth and Lars Bishop
		float twoDimSq = 2.0f * (scale * scale);
		float inertialTensorFactor = m_cubeMass * 1.0f / 12.0f * twoDimSq;
		float[] inertialTensor = {
			inertialTensorFactor, 0.0f, 0.0f,
			0.0f, inertialTensorFactor, 0.0f,
			0.0f, 0.0f, inertialTensorFactor
		};
		float[] inverseInertialTensor;
		GPUPhysics.Invert(ref inertialTensor, out inverseInertialTensor);
		float[] quickInverseInertialTensor = {
			1.0f/inertialTensorFactor, 0.0f, 0.0f,
			0.0f, 1.0f/inertialTensorFactor, 0.0f,
			0.0f, 0.0f, 1.0f/inertialTensorFactor
		};
		m_computeShader.SetFloats("inertialTensor", inertialTensor);
		m_computeShader.SetFloats("inverseInertialTensor", quickInverseInertialTensor);



		// initialize buffers
		// initial relative positions
		// super dependent on 8/rigid body
		particleInitialRelativePositions = new Vector3[n_particles];
		Vector3[] particleInitialsSmall = new Vector3[particlesPerBody];
		int initialRelativePositionIterator = 0;
		float centerer = scale * -0.5f + particleDiameter*0.5f;
		Vector3 centeringOffset = new Vector3(centerer, centerer, centerer);
		for (int xIter = 0; xIter < particlesPerEdge; xIter++) {
			for (int yIter = 0; yIter < particlesPerEdge; yIter++) {
				for (int zIter = 0; zIter < particlesPerEdge; zIter++) {
					if (xIter == 0 || xIter == (particlesPerEdge-1) || yIter == 0 || yIter == (particlesPerEdge-1) || zIter == 0 || zIter == (particlesPerEdge-1)) {
						particleInitialsSmall[initialRelativePositionIterator] = centeringOffset + new Vector3(xIter*particleDiameter, yIter*particleDiameter, zIter*particleDiameter);
						initialRelativePositionIterator++;
					}
				}
			}
		}
		for (int i = 0; i < particleInitialRelativePositions.Length; i++) {
			particleInitialRelativePositions[i] = particleInitialsSmall[i % particlesPerBody];
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
		m_computeShader.SetBuffer(m_kernel_computeMomenta, "rigidBodyQuaternions", m_rigidBodyQuaternions);

		// kernel 5 Compute Position and Rotation
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyVelocities", m_rigidBodyVelocities);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyAngularVelocities", m_rigidBodyAngularVelocities);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyPositions", m_rigidBodyPositions);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "rigidBodyQuaternions", m_rigidBodyQuaternions);
		m_computeShader.SetBuffer(m_kernel_computePositionAndRotation, "inverseInertialMatrices", m_rigidBodyInertialTensors);

		// kernel 6 Save Previous Position and Rotation
		m_computeShader.SetBuffer(m_kernelSavePreviousPositionAndRotation, "rigidBodyPositions", m_rigidBodyPositions);
		m_computeShader.SetBuffer(m_kernelSavePreviousPositionAndRotation, "rigidBodyQuaternions", m_rigidBodyQuaternions);
		m_computeShader.SetBuffer(m_kernelSavePreviousPositionAndRotation, "previousRigidBodyPositions", m_previousRigidBodyPositions);
		m_computeShader.SetBuffer(m_kernelSavePreviousPositionAndRotation, "previousRigidBodyQuaternions", m_previousRigidBodyQuaternions);
		// Setup Indirect Renderer
		uint[] sphereArgs = new uint[] { sphereMesh.GetIndexCount(0), (uint)n_particles, 0, 0, 0 };
		m_bufferWithSphereArgs = new ComputeBuffer(1, sphereArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		m_bufferWithSphereArgs.SetData(sphereArgs);

		uint[] lineArgs = new uint[] { lineMesh.GetIndexCount(0), (uint)n_particles, 0, 0, 0 };
		m_bufferWithLineArgs = new ComputeBuffer(1, lineArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		m_bufferWithLineArgs.SetData(lineArgs);

		cubeMaterial.SetBuffer("positions", m_rigidBodyPositions);
		cubeMaterial.SetBuffer("previousPositions", m_previousRigidBodyPositions);

		cubeMaterial.SetBuffer("quaternions", m_rigidBodyQuaternions);
		cubeMaterial.SetBuffer("previousQuaternions", m_previousRigidBodyQuaternions);

		sphereMaterial.SetBuffer("positions", m_particlePositions);
		sphereMaterial.SetVector("scale", new Vector4(particleDiameter*0.5f, particleDiameter*0.5f, particleDiameter*0.5f, 1.0f));
		lineMaterial.SetBuffer("positions", m_particlePositions);
		lineMaterial.SetBuffer("vectors", m_particleVelocities);

		// Setup Command Buffer
		m_commandBuffer = new CommandBuffer();
		m_commandBuffer.BeginSample("GenerateParticleValues");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_generateParticleValues, m_threadGroupsPerRigidBody, 1, 1);
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
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_computeMomenta, m_threadGroupsPerRigidBody, 1, 1);
		m_commandBuffer.EndSample("ComputeMomenta");

		m_commandBuffer.BeginSample("ComputePositions");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernel_computePositionAndRotation, m_threadGroupsPerRigidBody, 1, 1);
		m_commandBuffer.EndSample("ComputePositions");

		m_commandBuffer.BeginSample("SavePreviousPositionAndRotation");
		m_commandBuffer.DispatchCompute(m_computeShader, m_kernelSavePreviousPositionAndRotation, m_threadGroupsPerRigidBody, 1, 1);
		m_commandBuffer.EndSample("SavePreviousPositionAndRotation");
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
	public Matrix4x4 inverseInertialMatrix;
	void Update() {
		
		if (m_bufferWithArgs == null || m_debugWireframe != m_lastDebugWireframe) {
			uint indexCountPerInstance = cubeMesh.GetIndexCount(0);
			uint instanceCount = (uint)total;
			uint startIndexLocation = 0;
			uint baseVertexLocation = 0;
			uint startInstanceLocation = 0;
			uint[] args = new uint[] { indexCountPerInstance, instanceCount, startIndexLocation, baseVertexLocation, startInstanceLocation };
			m_bufferWithArgs = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
			m_bufferWithArgs.SetData(args);
			m_lastDebugWireframe = m_debugWireframe;
		}
		if (frameCounter++ < 10) {
			return;
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


		/*
		timestep adopted from the appreciated writings of Glenn Fiedler
		https://gafferongames.com/post/fix_your_timestep/

		accumulator += frameTime;

        while ( accumulator >= dt )
        {
            previousState = currentState;
            integrate( currentState, t, dt );
            t += dt;
            accumulator -= dt;
        }

        const double alpha = accumulator / dt;

        State state = currentState * alpha + 
            previousState * ( 1.0 - alpha );
        */
		ticker += Time.deltaTime;
		float _dt = 1.0f / tick_rate;
		while (ticker >= _dt) {
			ticker -= _dt;
			m_computeShader.SetFloat(m_deltaTimeShaderProperty, dt);

			Graphics.ExecuteCommandBuffer(m_commandBuffer);
		}
		float blendAlpha = ticker / _dt;
		cubeMaterial.SetFloat("blendAlpha", blendAlpha);
		//		#if (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) // Command Buffer DrawMeshInstancedIndirect doesnt work on my mac
		
		Graphics.DrawMeshInstancedIndirect(cubeMesh, 0, cubeMaterial, m_bounds, m_bufferWithArgs);
		if (m_debugWireframe) {
			m_computeShader.SetFloat("particleDiameter", scale / particlesPerEdge);
			m_computeShader.SetFloat("springCoefficient", springCoefficient);
			m_computeShader.SetFloat("dampingCoefficient", dampingCoefficient);
			m_computeShader.SetFloat("frictionCoefficient", frictionCoefficient);
			m_computeShader.SetFloat("angularFrictionCoefficient", angularFrictionCoefficient);
			m_computeShader.SetFloat("gravityCoefficient", gravityCoefficient);
			m_computeShader.SetFloat("tangentialCoefficient", tangentialCoefficient);
			m_computeShader.SetFloat("angularForceScalar", angularForceScalar);
			m_computeShader.SetFloat("linearForceScalar", linearForceScalar);
			int particlesPerBody = 8;
			m_computeShader.SetFloat("particleMass", m_cubeMass / particlesPerBody);
			/*
			m_rigidBodyInertialTensors.GetData(rigidBodyInertialTensors);
			string floatString = "";
			for (int i = 0; i < rigidBodyInertialTensors.Length; i++) {
				if (i % 3 == 0) {
					floatString += "\n";		
				}
				floatString += string.Format("\t{0}", rigidBodyInertialTensors[i].ToString());
			}
			Debug.Log("[GPUPhysics] inertialTensor:\n" + floatString);
			*/

			Graphics.DrawMeshInstancedIndirect(sphereMesh, 0, sphereMaterial, m_bounds, m_bufferWithSphereArgs);
			//lineMaterial.SetBuffer("positions", m_particlePositions);
			//lineMaterial.SetBuffer("vectors", m_particleVelocities);		
			
			lineMaterial.SetBuffer("positions", m_rigidBodyPositions);
			lineMaterial.SetBuffer("vectors", m_rigidBodyAngularVelocities);
			Graphics.DrawMeshInstancedIndirect(lineMesh, 0, lineMaterial, m_bounds, m_bufferWithLineArgs);
			m_rigidBodyQuaternions.GetData(quaternionArray);
			foreach(var r in comparisonCubes) {
				Debug.DrawLine(r.transform.position, r.transform.position + 10*r.angularVelocity, Color.green, Time.deltaTime*2);
				m_comparisonQuaternion = r.transform.rotation;
			}
			
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

		m_previousRigidBodyQuaternions.Release();
		m_previousRigidBodyPositions.Release();
		m_rigidBodyInertialTensors.Release();
		m_debugParticleIds.Release();

		if (m_bufferWithSphereArgs != null) {
			m_bufferWithSphereArgs.Release();
		}
		if (m_bufferWithLineArgs != null) {
			m_bufferWithLineArgs.Release();
		}
		if (m_bufferWithArgs != null) {
			m_bufferWithArgs.Release();
		}
	}

	public static int M(int row, int column) {
		return (row-1) * 3 + (column-1);
	}
	public static void Invert(ref float[] value, out float[] result) {
		float d11 = value[M(2,2)] * value[M(3,3)] + value[M(2,3)] * -value[M(3,2)];
		float d12 = value[M(2,1)] * value[M(3,3)] + value[M(2,3)] * -value[M(3,1)];
		float d13 = value[M(2,1)] * value[M(3,2)] + value[M(2,2)] * -value[M(3,1)];

		float det = value[M(1,1)] * d11 - value[M(1,2)] * d12 + value[M(1,3)] * d13;
		result = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

		if (Mathf.Abs(det) == 0.0f) {
			return;
		}

		det = 1f / det;

		float d21 = value[M(1, 2)] * value[M(3, 3)] + value[M(1, 3)] * -value[M(3, 2)];
		float d22 = value[M(1, 1)] * value[M(3, 3)] + value[M(1, 3)] * -value[M(3, 1)];
		float d23 = value[M(1, 1)] * value[M(3, 2)] + value[M(1, 2)] * -value[M(3, 1)];

		float d31 = (value[M(1, 2)] * value[M(2, 3)]) - (value[M(1, 3)] * value[M(2, 2)]);
		float d32 = (value[M(1, 1)] * value[M(2, 3)]) - (value[M(1, 3)] * value[M(2, 1)]);
		float d33 = (value[M(1, 1)] * value[M(2, 2)]) - (value[M(1, 2)] * value[M(2, 1)]);

		result[M(1,1)] = +d11 * det; result[M(1,2)] = -d21 * det; result[M(1,3)] = +d31 * det;
		result[M(2,1)] = -d12 * det; result[M(2,2)] = +d22 * det; result[M(2,3)] = -d32 * det;
		result[M(3,1)] = +d13 * det; result[M(3,2)] = -d23 * det; result[M(3,3)] = +d33 * det;
	}
}