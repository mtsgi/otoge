using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace OtoFuda.player
{
	public enum PlayerGripState
	{
		NONE,
		GRIP,
		RELEASE,
	}
	
	public class GetPlayerGripGesture : MonoBehaviour
	{ 
		[SerializeField] private LeapProvider targetProvider;
		
		[Range(0,1)]
		[SerializeField] private int playerID = 0;


		public static Action<int, PlayerGripState> OnGetPlayerGripGesture;
		
		internal PlayerGripState _PlayerGripState = PlayerGripState.NONE;

		
		private void OnEnable()
		{
			CustomHandEnableDisable.OnGetPlayerHandFinish += OnGetPlayerHandFinish;
		}
		

		private void OnGetPlayerHandFinish(int _playerID)
		{
			_PlayerGripState = PlayerGripState.RELEASE;
			OnGetPlayerGripGesture?.Invoke(playerID, _PlayerGripState);
		}

		private void Update()
		{
			var extendFinger = 0;
			if (targetProvider == null) return;
			if (targetProvider.CurrentFrame.Hands.Count == 0)
			{
				_PlayerGripState = PlayerGripState.RELEASE;
				return;
			}
			
			for (int i = 0; i < targetProvider.CurrentFrame.Hands[0].Fingers.Count; i++)
			{
				if (targetProvider.CurrentFrame.Hands[0].Fingers[i].IsExtended)
				{
					extendFinger += 1;
				}
			}
			
//			Debug.Log(targetProvider);

			if (extendFinger == 0 && _PlayerGripState == PlayerGripState.NONE)
			{
				_PlayerGripState = PlayerGripState.GRIP;
				OnGetPlayerGripGesture?.Invoke(playerID, _PlayerGripState);
				//Debug.Log(playerID+":グー！");
			}
			else if (extendFinger >= 3 && _PlayerGripState == PlayerGripState.GRIP)
			{
				_PlayerGripState = PlayerGripState.RELEASE;
				OnGetPlayerGripGesture?.Invoke(playerID, _PlayerGripState);
			//	Debug.Log(playerID+":パー！");
			}
			else if (_PlayerGripState == PlayerGripState.RELEASE)
			{
				_PlayerGripState = PlayerGripState.NONE;
				OnGetPlayerGripGesture?.Invoke(playerID, _PlayerGripState);
			//	Debug.Log(playerID+":なんもなし");
			}

		}
	}

}
