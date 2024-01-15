using System.Collections.Generic;
using Treasure;
using UIFW;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 虚拟房主
/// </summary>
public class ParlorVirtualOwnerController : ISingleton
{
	public Room Room;
	public RoomUserInfo OwnerInfo;
	private bool isOwner;

	/*------------------------离线邀请------------------------*/
	//是否邀请过，每次进入房间可以邀请一次
	public bool Invated;

	public void Init()
	{
		MessageCenter.AddMsgListener("OnVirtualOwnerClick", (p) =>
		{
			if (Invated)
			{
				return;
			}

			imgInviteTip.SetActive(false);
			btnInvite.gameObject.SetActive(true);
		});
	}

	/// <summary>
	/// 进入房间
	/// </summary>
	/// <param name="curRoomData"></param>
	public void EnterRoom()
	{
		isOwner = OwnerInfo.UserId == ManageMentClass.DataManagerClass.userId;
		if (isOwner)
		{
			return;
		}

		//判断房主是否在线
		var ownerOnLine = false;
		foreach (var roomUserInfo in Room.UserList)
		{
			if (roomUserInfo.UserId == OwnerInfo.UserId)
			{
				ownerOnLine = true;
				break;
			}
		}

		//房主在线
		if (ownerOnLine)
		{
			return;
		}

		CreateAvatar(this.OwnerInfo);
	}

	/// <summary>
	/// 离开房间
	/// </summary>
	public void LeaveRoom()
	{
		Invated = false;
		DestroyAvatar();
	}

	public void OnOtherEnterTreasurePush(OtherEnterTreasurePush enterPush)
	{
		//房主上线, 销毁虚拟形象
		if (enterPush.NewUserInfo.UserId == OwnerInfo.UserId)
		{
			DestroyAvatar();
		}
	}

	public void OnOtherLeaveTreasurePush(OtherLeaveTreasurePush leavePush)
	{
		//房主上线, 销毁虚拟形象
		//房主离开了，创建虚拟形象
		if (leavePush.FromUserId == this.OwnerInfo.UserId)
		{
			Singleton<ParlorController>.Instance.TryGetPlayerImp(leavePush.FromUserId, out var playerControllerImp);
			if (playerControllerImp == null)
			{
				return;
			}

			CreateAvatar(playerControllerImp.UserInfo);
		}
	}

	private GameObject go;

	private GameObject imgInviteTip;
	private GameObject btnInvite;
	private GameObject imgInvited;

	private RoomUserInfo virtualOwnerInfo;
	private GameObject hudPanel;

	private void CreateAvatar(RoomUserInfo ownerInfo)
	{
		virtualOwnerInfo = new RoomUserInfo(ownerInfo);
		var realUseId = ownerInfo.UserId;
		virtualOwnerInfo.UserId = realUseId * 10000;
		go = AvatarManager.Instance().GetOtherPlayerPreFun(virtualOwnerInfo);
		go.name = $"virtual_owner_{virtualOwnerInfo.UserId}";
		go.SetActive(true);
		go.transform.localPosition = new Vector3(5.865f, 0.2f, -5.826f);
		go.transform.localRotation = Quaternion.Euler(new Vector3(0, -7.35f, 0));
		go.AddComponent<VirtualOwnerEventClick>();

		if (go.GetComponent<EventItemClick>() != null)
		{
			Object.Destroy(go.GetComponent<EventItemClick>());
		}

		if (go.GetComponent<PlayerItem>() != null)
		{
			Object.Destroy(go.GetComponent<PlayerItem>());
		}

		var renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		var material = Resources.Load<Material>("Materials/Disconnect");
		foreach (var renderer in renderers)
		{
			Material[] materials = new Material[renderer.materials.Length];
			for (int i = 0; i < materials.Length; i++)
			{
				materials[i] = material;
			}

			renderer.materials = materials;
		}

		var animator = go.GetComponent<Animator>();
		animator.Play("LookAround");

		//CombineObject(go, go.GetComponentsInChildren<SkinnedMeshRenderer>(), false);
		
		AddHudPanel(go);
	}

	private void DestroyAvatar()
	{
		if (virtualOwnerInfo == null)
		{
			return;
		}

		AvatarManager.Instance().RecyclePlayerPreFun(virtualOwnerInfo.UserId);
		virtualOwnerInfo = null;

		if (hudPanel != null)
		{
			Object.Destroy(hudPanel);
			hudPanel = null;
		}
	}

	private void AddHudPanel(GameObject go)
	{
		hudPanel = ResourcesMgr.GetInstance().LoadAsset("Prefabs/ScenePos/VirtualOwnerPanel", true);
		hudPanel.name = "VirtualOwnerPanel";
		Transform parent = go.transform.Find("Hud");
		hudPanel.transform.SetParent(parent);
		hudPanel.transform.localPosition = Vector3.zero;
		hudPanel.transform.localScale = Vector3.one * 0.001f;
		hudPanel.GetComponent<Canvas>().worldCamera = Camera.main;

		imgInviteTip = hudPanel.transform.Find("ChatBubble/imgInviteTip").gameObject;
		btnInvite = hudPanel.transform.Find("ChatBubble/btnInvite").gameObject;
		imgInvited = hudPanel.transform.Find("ChatBubble/imgInvited").gameObject;

		btnInvite.GetComponent<Button>().onClick.AddListener(onBtnInviteClick);

		btnInvite.gameObject.SetActive(false);
		imgInviteTip.gameObject.SetActive(!Invated);
		imgInvited.gameObject.SetActive(Invated);
	}

	private void onBtnInviteClick()
	{
		var from_user_id = ManageMentClass.DataManagerClass.userId;
		var to_user_id = OwnerInfo.UserId;
		var land_id = ManageMentClass.DataManagerClass.landId;
		MessageManager.GetInstance().SendSummonBackReq(from_user_id, to_user_id, land_id, () =>
		{
			btnInvite.gameObject.SetActive(false);
			imgInvited.gameObject.SetActive(true);
		});
	}

	/// <summary>
	/// 仅用于合并材料
	/// </summary>
	private const int COMBINE_TEXTURE_MAX = 512;

	private const string COMBINE_DIFFUSE_TEXTURE = "_MainTex";

	/// <summary>
	/// 将 SkinnedMeshRenderers 合并在一起共享一个骨架
	/// 合并材料将减少渲染管道，但是会增加内存空间
	/// </summary>
	/// <param name="skeleton">把网格与这个骨架结合起来(a gameobject)</param>
	/// <param name="meshes">需要合并网格</param>
	/// <param name="combine">是否合并材料</param>
	public void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
	{

		// 取出骨骼的全部骨骼
		List<Transform> transforms = new List<Transform>();
		transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

		List<Material> materials = new List<Material>(); //the list of materials
		List<CombineInstance> combineInstances = new List<CombineInstance>(); //the list of meshes
		List<Transform> bones = new List<Transform>(); //the list of bones

		// 以下信息仅用于合并材料(bool combine = true)
		List<Vector2[]> oldUV = null;
		Material newMaterial = null;
		Texture2D newDiffuseTex = null;

		//收集信息网格
		for (int i = 0; i < meshes.Length; i++)
		{
			SkinnedMeshRenderer smr = meshes[i];
			materials.AddRange(smr.materials); // Collect materials
			//  选择网格
			for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
			}

			// 选择骨骼
			for (int j = 0; j < smr.bones.Length; j++)
			{
				int tBase = 0;
				for (tBase = 0; tBase < transforms.Count; tBase++)
				{
					if (smr.bones[j].name.Equals(transforms[tBase].name))
					{
						bones.Add(transforms[tBase]);
						break;
					}
				}
			}
		}

		// 合并材质
		if (combine)
		{
			newMaterial = new Material(Shader.Find("Mobile/Diffuse"));
			oldUV = new List<Vector2[]>();
			// 合并图片
			List<Texture2D> Textures = new List<Texture2D>();
			for (int i = 0; i < materials.Count; i++)
			{
				Textures.Add(materials[i].GetTexture(COMBINE_DIFFUSE_TEXTURE) as Texture2D);
			}

			newDiffuseTex = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
			Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
			newMaterial.mainTexture = newDiffuseTex;

			// 重置紫外线
			Vector2[] uva, uvb;
			for (int j = 0; j < combineInstances.Count; j++)
			{
				uva = (Vector2[])(combineInstances[j].mesh.uv);
				uvb = new Vector2[uva.Length];
				for (int k = 0; k < uva.Length; k++)
				{
					uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
				}

				oldUV.Add(combineInstances[j].mesh.uv);
				combineInstances[j].mesh.uv = uvb;
			}
		}

		//创建新的SkinnedMeshRenderer
		SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
		if (oldSKinned != null)
		{

			GameObject.DestroyImmediate(oldSKinned);
		}

		SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
		r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false); // 组合网格
		r.bones = bones.ToArray(); //使用新骨骼
		if (combine)
		{
			r.material = newMaterial;
			for (int i = 0; i < combineInstances.Count; i++)
			{
				combineInstances[i].mesh.uv = oldUV[i];
			}
		}
		else
		{
			r.materials = materials.ToArray();
		}
	}
}
