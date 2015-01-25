using UnityEngine;


/// <summary>
/// Holds general constants that don't fit anywhere else.
/// </summary>
public class GameConstants : MonoBehaviour
{
	private static GameConstants instance;

	
	public static float RockAttackDist { get { return instance._RockAttackDist; }  }
	public float _RockAttackDist = 0.05f;


	public static GameObject RockPrefab { get { return instance._RockPrefab; } }
	public GameObject _RockPrefab;

	public static GameObject PaperPrefab { get { return instance._PaperPrefab; } }
	public GameObject _PaperPrefab;

	public static GameObject ScissorsPrefab { get { return instance._ScissorsPrefab; } }
	public GameObject _ScissorsPrefab;

	
	public static AudioClip RockAttackAudio { get { return instance._RockAttackAudio; } }
	public AudioClip _RockAttackAudio;
	public static AudioClip PaperAttackAudio { get { return instance._PaperAttackAudio; } }
	public AudioClip _PaperAttackAudio;
	public static AudioClip ScissorsAttackAudio { get { return instance._ScissorsAttackAudio; } }
	public AudioClip _ScissorsAttackAudio;


	void Awake()
	{
		instance = this;
	}
}