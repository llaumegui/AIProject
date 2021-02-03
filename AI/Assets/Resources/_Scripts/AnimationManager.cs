using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
	public SpriteRenderer ShadowRenderer;
	public float Framerate = .1f;
	public AnimationsSprites[] AnimationList;

	[System.Serializable]
	public class AnimationsSprites
	{
		public AnimationManager.States State;
		public List<Sprite> Sprites;
		public float CustomFramerate;
		public bool Loop;

		[Space]
		public bool DebugActivate;
	}



	int _index;
	float _timer;
	SpriteRenderer _renderer;

	List<Sprite> _sprites;
	bool _looping;
	float _framerate;
	bool _hasShadow;


	public enum States
	{
		idle,
		walk,
		attack,
		hurt,

		debug,
	}

	void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_sprites = SetSprites(States.idle);

		if(ShadowRenderer != null)
			_hasShadow = true;
	}

	// Update is called once per frame
	void Update()
	{
		DisplaySprite();

		CheckDebug();
	}

	void DisplaySprite()
	{
		_timer += Time.deltaTime;
		if (_timer > _framerate)
		{
			_timer = 0;
			_index = (_index + 1) % _sprites.Count;

			if (!_looping && _index == 0)
			{
				_sprites = SetSprites(States.idle);
			}

			_renderer.sprite = _sprites[_index];
			if (_hasShadow)
				ShadowRenderer.sprite = _sprites[_index];
		}
	}

	public void Play(States state)
	{
		_looping = false;
		_sprites = SetSprites(state);
	}

	public void PlayLoop(States state)
	{
		Play(state);
		_looping = true;
	}

	void CheckDebug()
	{
		States stateDebug = States.debug;
		foreach (AnimationsSprites anim in AnimationList)
		{
			if (anim.DebugActivate && stateDebug == States.debug)
				stateDebug = anim.State;

			anim.DebugActivate = false;
		}

		if(stateDebug != States.debug)
			_sprites = SetSprites(stateDebug);
	}

	List<Sprite> SetSprites(States state = States.debug, bool debugLoop = false)
	{
		AnimationsSprites animSelected = null;

		foreach(AnimationsSprites anim in AnimationList)
		{
			if (anim.State == state)
				animSelected = anim;
		}

		if(animSelected == null)
			animSelected = AnimationList[0];

		ResetAnim();

		if(debugLoop)
			_looping = animSelected.Loop;


		if (animSelected.CustomFramerate > 0)
			_framerate = animSelected.CustomFramerate;
		else
			_framerate = Framerate;

		return animSelected.Sprites;
	}

	void ResetAnim()
	{
		_index = 0;
		_timer = 0;
	}
}
