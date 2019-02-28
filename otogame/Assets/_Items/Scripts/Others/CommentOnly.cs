using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentOnly : MonoBehaviour
{
    [Header("このスクリプトは説明専用のスクリプトです。")] 
    [SerializeField] [TextArea(1,4)] private string comment = "ここにスクリプトとかの説明文を書いてください\n";

}
