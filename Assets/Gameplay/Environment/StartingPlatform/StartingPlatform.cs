using Gameplay.Player;
using UnityEngine;

public class StartingPlatform : MonoBehaviour
{
    public new Collider2D collider;
    public new Animator animation;

    private bool hidden = false;

	void Awake()
	{
        animation.enabled = false;
        Character character = FindAnyObjectByType<Character>();
        if (character)
            character.grappled.AddListener(HidePlatform);
	}

	public void HidePlatform()
    {
        if (hidden) return;

        animation.enabled = true;
        collider.enabled = false;
        hidden = true;
    }
}
