/*
 * Copyright (c) 2020 LG Electronics Inc.
 *
 * SPDX-License-Identifier: MIT
 */

using UE = UnityEngine;
using UEAI = UnityEngine.AI;

namespace SDF
{
	namespace Helper
	{
		public class Model : Base
		{
			private float CarvingMoveThreshold = 0.1f;
			private float CarvingTimeToStationary = 0.3f;

			public bool hasRootArticulationBody;

			[UE.Header("SDF Properties")]
			public bool isStatic;

			void Start()
			{
				if (isStatic)
				{
					// if parent model has static option, make it all static in children
					ConvertToStaticLink();
				}
				else
				{
					if (hasRootArticulationBody)
					{
						var carveOnlyStationary = IsRobotModel() ? false : true;
						AddNavMeshObstalce(carveOnlyStationary);
					}
				}
			}

			private void AddNavMeshObstalce(in bool carveOnlyStationary)
			{
				var bounds = new UE.Bounds();
				var renderers = transform.GetComponentsInChildren<UE.Renderer>();
				for (var i = 0; i < renderers.Length; i++)
				{
					bounds.Encapsulate(renderers[i].bounds.size);
				}

				var navMeshObstacle = gameObject.AddComponent<UEAI.NavMeshObstacle>();
				navMeshObstacle.carving = true;
				navMeshObstacle.carveOnlyStationary = carveOnlyStationary;
				navMeshObstacle.carvingMoveThreshold = CarvingMoveThreshold;
				navMeshObstacle.carvingTimeToStationary = CarvingTimeToStationary;
				navMeshObstacle.size = transform.rotation * bounds.size;
			}

			private bool IsRobotModel()
			{
				var artBodies = GetComponentsInChildren<UE.ArticulationBody>();
				var artMaxIndex = 0;
				foreach (var artBody in artBodies)
				{
					if (artBody.index > artMaxIndex)
					{
						artMaxIndex = artBody.index;
					}
				}

				return (artMaxIndex > 1) ? true : false;
			}

			private void ConvertToStaticLink()
			{
				gameObject.isStatic = true;

				foreach (var childGameObject in GetComponentsInChildren<UE.Transform>())
				{
					childGameObject.gameObject.isStatic = true;
				}

				foreach (var childArticulationBody in GetComponentsInChildren<UE.ArticulationBody>())
				{
					if (childArticulationBody.isRoot)
					{
						childArticulationBody.immovable = true;
					}
				}
			}

			void LateUpdate()
			{
				SetPose(transform.localPosition, transform.localRotation, 1);
			}
		}
	}
}