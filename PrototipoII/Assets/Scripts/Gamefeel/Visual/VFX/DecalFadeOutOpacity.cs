using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class DecalFadeOutOpacity : MonoBehaviour
{
    [SerializeField]
    private float visibleDuration = 20;
    [SerializeField]
    private float fadeDuration = 15;

    private DecalProjector _decalProjector;
   
    private void OnEnable()
    {
        _decalProjector = GetComponent<DecalProjector>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(visibleDuration);

        float elapsed = 0;
        float initialFactor = _decalProjector.fadeFactor;
       
        while(elapsed < 1)
        {
            _decalProjector.fadeFactor = Mathf.Lerp(initialFactor, 0, elapsed);
            elapsed += Time.deltaTime / fadeDuration;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
