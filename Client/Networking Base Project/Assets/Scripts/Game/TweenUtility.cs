using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenUtility : MonoBehaviour
{
	public static TweenUtility Instance;

	public AnimationCurve Liner;

	public AnimationCurve EaseInSine;
	public AnimationCurve EaseOutSine;

	public AnimationCurve EaseInQuart;
	public AnimationCurve EaseOutQuart;

	public AnimationCurve EaseInBounce;
	public AnimationCurve EaseOutBounce;

	public AnimationCurve EaseInElastic;
	public AnimationCurve EaseOutElastic;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			Destroy(this);
		}
	}

	IEnumerator coEase_Transform(Transform target, AnimationCurve curve,
		Vector3 axis, float start, float end, float duration, float delay, Action action = null)
	{
		// The starting position
		Vector3 pos = target.position;

		// Wait for the delay
		yield return new WaitForSecondsRealtime(delay);

		float counter = 0.0f;
		while (counter < duration)
		{
			// Add to the timer so we can calculate what percent of the animation we are through
			counter += Time.deltaTime;

			float value = curve.Evaluate(counter / duration);

			if (axis == new Vector3(1, 0, 0))
				target.position = new Vector3(Mathf.LerpUnclamped(start, end, value), pos.y, pos.z);

			if (axis == new Vector3(0, 1, 0))
				target.position = new Vector3(pos.x, Mathf.LerpUnclamped(start, end, value), pos.z);

			if (axis == new Vector3(0, 0, 1))
				target.position = new Vector3(pos.x, pos.y, Mathf.LerpUnclamped(start, end, value));

			yield return null;
		}

		if (action != null) action.Invoke();
	}

	IEnumerator coEase_Rotation(Transform target, AnimationCurve curve,
		Vector3 axis, float start, float end, float duration, float delay, Action action = null)
	{
		// The starting position
		Vector3 euler = target.eulerAngles;

		// Wait for the delay
		yield return new WaitForSecondsRealtime(delay);

		float counter = 0.0f;
		while (counter < duration)
		{
			// Add to the timer so we can calculate what percent of the animation we are through
			counter += Time.deltaTime;

			float value = curve.Evaluate(counter / duration);

			if (axis == new Vector3(1, 0, 0))
				target.eulerAngles = new Vector3(Mathf.LerpUnclamped(start, end, value), euler.y, euler.z);

			if (axis == new Vector3(0, 1, 0))
				target.eulerAngles = new Vector3(euler.x, Mathf.LerpUnclamped(start, end, value), euler.z);

			if (axis == new Vector3(0, 0, 1))
				target.eulerAngles = new Vector3(euler.x, euler.y, Mathf.LerpUnclamped(start, end, value));

			yield return null;
		}

		if (action != null) action.Invoke();
	}

	IEnumerator coEase_Scale(Transform target, AnimationCurve curve,
		Vector3 axis, float start, float end, float duration, float delay, Action action = null)
	{
		// The starting position
		Vector3 scale = target.localScale;

		// Wait for the delay
		yield return new WaitForSecondsRealtime(delay);

		float counter = 0.0f;
		while (counter < duration)
		{
			// Add to the timer so we can calculate what percent of the animation we are through
			counter += Time.deltaTime;

			float value = curve.Evaluate(counter / duration);

			if (axis .x == 1)
				target.localScale = new Vector3(Mathf.LerpUnclamped(start, end, value), target.localScale.y, target.localScale.z);

			if (axis.y == 1)
				target.localScale = new Vector3(target.localScale.x, Mathf.LerpUnclamped(start, end, value), target.localScale.z);

			if (axis.z == 1)
				target.localScale = new Vector3(target.localScale.x, target.localScale.y, Mathf.LerpUnclamped(start, end, value));

			yield return null;
		}

		if (action != null) action.Invoke();
	}

	IEnumerator coEase_Size(RectTransform target, AnimationCurve curve, 
		Vector2 axis, float start, float end, float duration, float delay, Action action = null)
	{
		// The starting position
		float width = target.sizeDelta.x;

		// Wait for the delay
		yield return new WaitForSecondsRealtime(delay);

		float counter = 0.0f;
		while (counter < duration)
		{
			// Add to the timer so we can calculate what percent of the animation we are through
			counter += Time.deltaTime;

			float value = curve.Evaluate(counter / duration);

			if (axis == new Vector2(1, 0))
				target.sizeDelta = new Vector2(Mathf.LerpUnclamped(start, end, value), target.sizeDelta.y);

			if (axis == new Vector2(0, 1))
				target.sizeDelta = new Vector2(target.sizeDelta.x, Mathf.LerpUnclamped(start, end, value));

			yield return null;
		}

		if (action != null) action.Invoke();
	}

	// Transform
	#region Transform
	public void Ease_Transform_LinerX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void Ease_Transform_LinerY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void Ease_Transform_LinerZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Transform_SineX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_SineY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_SineZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, Liner, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Transform_SineX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutSine, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_SineY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutSine, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_SineZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutSine, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Transform_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Transform_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Transform_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Transform_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Transform_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Transform_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseInElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Transform_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Transform_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Transform(target, EaseOutElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));
	#endregion

	// Scale
	#region Scale
	public void Ease_Scale_LinerX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, Liner, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void Ease_Scale_LinerY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, Liner, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void Ease_Scale_LinerZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, Liner, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Scale_SineX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutSine, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_SineY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutSine, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_SineZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutSine, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Scale_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Scale_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Scale_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));
	public void EaseIn_Scale_BounceAll(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInBounce, new Vector3(1, 1, 1), start, end, duration, delay, action));

	public void EaseOut_Scale_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));
	public void EaseOut_Scale_BounceAll(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInBounce, new Vector3(1, 1, 1), start, end, duration, delay, action));

	public void EaseIn_Scale_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Scale_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));
	public void EaseIn_Scale_ElasticAll(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseInElastic, new Vector3(1, 1, 1), start, end, duration, delay, action));

	public void EaseOut_Scale_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Scale_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));
	public void EaseOut_Scale_ElasticAll(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Scale(target, EaseOutElastic, new Vector3(1, 1, 1), start, end, duration, delay, action));

	#endregion

	// Rotation
	#region Rotation
	public void Ease_Rotation_LinerX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, Liner, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void Ease_Rotation_LinerY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, Liner, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void Ease_Rotation_LinerZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, Liner, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Rotation_SineX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutSine, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_SineY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutSine, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_SineZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutSine, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Rotation_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Rotation_QuartX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutQuart, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_QuartY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutQuart, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_QuartZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutQuart, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Rotation_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Rotation_BounceX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutBounce, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_BounceY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutBounce, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_BounceZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutBounce, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseIn_Rotation_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseIn_Rotation_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseInElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));

	public void EaseOut_Rotation_ElasticX(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutElastic, new Vector3(1, 0, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_ElasticY(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutElastic, new Vector3(0, 1, 0), start, end, duration, delay, action));
	public void EaseOut_Rotation_ElasticZ(Transform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Rotation(target, EaseOutElastic, new Vector3(0, 0, 1), start, end, duration, delay, action));
	#endregion

	// Size
	#region Size
	public void Ease_Size_LinerAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, Liner, new Vector3(1, 1), start, end, duration, delay, action));
	public void Ease_Size_LinerWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, Liner, new Vector3(1, 0), start, end, duration, delay, action));
	public void Ease_Size_LinerHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, Liner, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseOut_Size_SineAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutSine, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseOut_Size_SineWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutSine, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseOut_Size_SineHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutSine, new Vector3(0, 1), start, end, duration, delay));

	public void EaseIn_Size_SineAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInSine, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseIn_Size_SineWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInSine, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseIn_Size_SineHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInSine, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseOut_Size_QuartAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutQuart, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseOut_Size_QuartWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutQuart, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseOut_Size_QuartHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutQuart, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseIn_Size_QuartAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInQuart, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseIn_Size_QuartWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInQuart, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseIn_Size_QuartHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInQuart, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseOut_Size_BounceAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutBounce, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseOut_Size_BounceWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutBounce, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseOut_Size_BounceHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutBounce, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseIn_Size_BounceAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInBounce, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseIn_Size_BounceWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInBounce, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseIn_Size_BounceHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInBounce, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseOut_Size_ElasticAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutElastic, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseOut_Size_ElasticWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutElastic, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseOut_Size_ElasticHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseOutElastic, new Vector3(0, 1), start, end, duration, delay, action));

	public void EaseIn_Size_ElasticAll(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInElastic, new Vector3(1, 1), start, end, duration, delay, action));
	public void EaseIn_Size_ElasticWidth(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInElastic, new Vector3(1, 0), start, end, duration, delay, action));
	public void EaseIn_Size_ElasticHeight(RectTransform target, float start, float end, float duration, float delay = 0, Action action = null) => StartCoroutine(coEase_Size(target, EaseInElastic, new Vector3(0, 1), start, end, duration, delay, action));

	#endregion
}
