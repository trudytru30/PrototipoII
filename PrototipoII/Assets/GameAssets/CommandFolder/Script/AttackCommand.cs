using Unity.Mathematics;
using UnityEngine;
using System.Collections;

public class AttackCommand : ICommand
{
    private readonly GameObject _weapon;
    private readonly GameObject _arm;
    private readonly MonoBehaviour _runner;

    private readonly float _duration;
    private readonly float _target;
    private readonly float _origin;

    private Coroutine _playing;

    public AttackCommand(GameObject arm, GameObject weapon, MonoBehaviour runner)
    {
        _arm = arm;
        _weapon = weapon;
        _runner = runner;

        var w = _weapon.GetComponent<Weapon>();
        if (w == null) 
            return;
        _origin   = w.Origin;
        _target   = w.Target;
        _duration = Mathf.Max(w.Duration, 0.0001f);
    }

    public void ExecuteAction()
    {
        StartPlay(_origin, _target);
    }

    public void UndoAction()
    {
        StartPlay(_target, _origin);
    }

    private void StartPlay(float from, float to)
    {
        if (_playing != null)
            _runner.StopCoroutine(_playing);

        _playing = _runner.StartCoroutine(PlayAttack(from, to));
    }

    private IEnumerator PlayAttack(float from, float to)
    {
        _weapon.SetActive(true);
        float elapsed = 0f;
        
        _arm.transform.localRotation = Quaternion.Euler(0, from, 0);
        
        var e = _arm.transform.localEulerAngles;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = math.saturate(elapsed / _duration);
            float y = math.lerp(from, to, t);
            _arm.transform.localRotation = Quaternion.Euler(e.x, y, e.z);
            yield return null;
        }

        _arm.transform.localRotation = Quaternion.Euler(e.x, to, e.z);

        _weapon.SetActive(false);
        _playing = null;
    }
}