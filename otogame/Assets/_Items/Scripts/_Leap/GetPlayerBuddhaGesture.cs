﻿using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

namespace OtoFuda.player
{
	public class GetPlayerBuddhaGesture : MonoBehaviour
	{
		[Range(0,1)]
		[SerializeField] private int playerID = 0;

		[SerializeField] private float judgePalmDistance = 0.01f;
		[SerializeField] private int frameGetFetchTime = 20;
		
		public static Action<int> OnGetPlayerBuddhaGesture;
		public static Action<int> OnReleasePlayerBuddhaPalm;
		
		private Controller _controller = new Controller();

		private bool isBuddhaGesture = false;

		private void OnEnable()
		{
			CustomHandEnableDisable.OnGetPlayerHandFinish += OnGetPlayerHandFinish;
		}
		

		private void OnGetPlayerHandFinish(int _playerID)
		{
			isBuddhaGesture = false;
		}


		//右手と左手がぶつかった瞬間に仏陀ジェスチャをしたと判定する
		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.tag == "RightHand")
			{
				isBuddhaGesture = true;
				//アクションを発火
				OnGetPlayerBuddhaGesture?.Invoke(playerID);
			}
		}

		private void OnCollisionStay(Collision other)
		{
			if (other.gameObject.tag == "RightHand")
			{
				var _frame = _controller.Frame();
			
				if (_frame.Hands.Count >= 2 && _controller.Frame(frameGetFetchTime) != null)
				{
					var nowFramePalmDistanceX = Mathf.Abs(_controller.Frame(0).Hands[0].PalmPosition.x -
					                                      _controller.Frame(0).Hands[1].PalmPosition.x);
					var beforeFramePalmDistanceX = Mathf.Abs(_controller.Frame(frameGetFetchTime).Hands[0].PalmPosition.x -
					                                         _controller.Frame(frameGetFetchTime).Hands[1].PalmPosition.x);

					var distanceDifference = Mathf.Abs(nowFramePalmDistanceX - beforeFramePalmDistanceX);
					

					if (isBuddhaGesture && distanceDifference > judgePalmDistance)
					{
						isBuddhaGesture = false;
						OnReleasePlayerBuddhaPalm?.Invoke(playerID);
						
					}
				}
			}
			
		}
		
	}

}
