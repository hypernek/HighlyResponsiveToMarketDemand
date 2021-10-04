using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ImageAnimator : MonoBehaviour
{
	public static UnityEvent event_singleAnimationCompleted = new UnityEvent();

	public Sprite[] spritesIdle;
	public Sprite[] spritesAttack;
	public Sprite[] spritesMove;

	private Sprite[] activeSprites;

	public Image image;
	public string animationToPlay;
	public int spritePerFrame = 6;
	public bool loop = true;
	public bool destroyOnEnd = false;

	private int index = 0;

	private int frame = 0;

	public void SwitchAnimation(string which)
	{
		if (which == "idle")
		{
			activeSprites = spritesIdle;

		}
		else if (which == "attack")
		{
			activeSprites = spritesAttack;
		}
		else if (which == "move")
		{
			activeSprites = spritesMove;
		}
	}

	void Start()
	{
		activeSprites = spritesIdle;
		SwitchAnimation(animationToPlay);
	}

	void Update()
	{
		if (!loop && index == activeSprites.Length) return;
		frame++;
		if (frame < spritePerFrame) return;
		image.sprite = activeSprites[index];
		frame = 0;
		index++;
		if (index >= activeSprites.Length)
		{
			if (loop) index = 0;
			if (destroyOnEnd) Destroy(gameObject);
		}

	}
}