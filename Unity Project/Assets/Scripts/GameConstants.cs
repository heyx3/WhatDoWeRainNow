using UnityEngine;


/// <summary>
/// Holds general constants that don't fit anywhere else.
/// </summary>
public class GameConstants : MonoBehaviour
{
	private static GameConstants instance;

	
	public static float RockAttackDist { get { return instance._RockAttackDist; }  }
	public float _RockAttackDist = 0.05f;
	
	public static float ScissorBulletSpeed { get { return instance._ScissorBulletSpeed; }  }
	public float _ScissorBulletSpeed = 0.05f;
	
	public static float PaperAttackDuration { get { return instance._PaperAttackDuration; }  }
	public float _PaperAttackDuration = 0.5f;

	
	public static float RockAttackDamage { get { return instance._RockAttackDamage; }  }
	public float _RockAttackDamage = 0.4f;
	
	public static float ScissorBulletDamage { get { return instance._ScissorBulletDamage; }  }
	public float _ScissorBulletDamage = 0.5f;
	
	public static float PaperAttackDamage { get { return instance._PaperAttackDamage; }  }
	public float _PaperAttackDamage = 0.275f;


	public static GameObject PaperParticles { get { return instance._PaperParticles; } }
	public GameObject _PaperParticles;

	public static GameObject ScissorsParticles { get { return instance._ScissorsParticles; } }
	public GameObject _ScissorsParticles;

	
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