using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace Gameplay.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Win Settings")]
        [SerializeField] private string _winMessage = "VICTORY!";
        [SerializeField] private Color _winColor = Color.green;

        [Header("Lose Settings")]
        [SerializeField] private string _loseMessage = "WASTED";
        [SerializeField] private Color _loseColor = Color.red;

        private Coroutine _animationCoroutine;

        public void ShowResult(bool isWin)
        {
            gameObject.SetActive(true);
            _resultText.text = isWin ? _winMessage : _loseMessage;
            
            _resultText.color = new Color(0, 0, 0, 0); 
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, 0.5f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
            
            if (isWin)
                _animationCoroutine = StartCoroutine(AnimateWinText());
            else
                _animationCoroutine = StartCoroutine(AnimateLoseText());
        }

        private IEnumerator AnimateWinText()
        {
            _resultText.ForceMeshUpdate();
            TMP_TextInfo textInfo = _resultText.textInfo;
            
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            int charCount = textInfo.characterCount;
            
            float[] scales = new float[charCount];
            float[] yOffsets = new float[charCount];
            Color32[] colors = new Color32[charCount];
            
            for (int i = 0; i < charCount; i++)
            {
                scales[i] = 0f;
                colors[i] = new Color32((byte)(_winColor.r * 255), (byte)(_winColor.g * 255), (byte)(_winColor.b * 255), 0);
            }
            
            for (int i = 0; i < charCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                int index = i;

                float delay = i * 0.08f;
                
                DOTween.To(() => scales[index], x => scales[index] = x, 1f, 0.4f).SetEase(Ease.OutBack).SetDelay(delay);
                DOTween.To(() => yOffsets[index], y => yOffsets[index] = y, 25f, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutSine).SetDelay(delay);
                DOTween.To(() => colors[index].a, a => { colors[index].a = (byte)a; }, 255, 0.2f).SetDelay(delay);
            }

            float duration = (charCount * 0.08f) + 0.5f;
            yield return AnimateMeshRoutine(textInfo, cachedMeshInfo, duration, scales, yOffsets, colors, null);
        }

        private IEnumerator AnimateLoseText()
        {
            _resultText.ForceMeshUpdate();
            TMP_TextInfo textInfo = _resultText.textInfo;
            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            int charCount = textInfo.characterCount;

            float[] yOffsets = new float[charCount];
            Color32[] colors = new Color32[charCount];
            float[] shakeIntensities = new float[charCount];

            for (int i = 0; i < charCount; i++)
            {
                yOffsets[i] = 80f;
                colors[i] = new Color32((byte)(_loseColor.r * 255), (byte)(_loseColor.g * 255), (byte)(_loseColor.b * 255), 0);
            }

            for (int i = 0; i < charCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;
                int index = i;
                float delay = i * 0.12f;
                
                DOTween.To(() => yOffsets[index], y => yOffsets[index] = y, 0f, 0.6f).SetEase(Ease.OutBounce).SetDelay(delay);
                DOTween.To(() => colors[index].a, a => { colors[index].a = (byte)a; }, 255, 0.3f).SetDelay(delay);
                DOTween.To(() => shakeIntensities[index], s => shakeIntensities[index] = s, 4f, 0.1f)
                    .SetLoops(15, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(delay + 0.6f);
            }

            float duration = (charCount * 0.12f) + 2.5f;
            yield return AnimateMeshRoutine(textInfo, cachedMeshInfo, duration, null, yOffsets, colors, shakeIntensities);
        }
        
        private IEnumerator AnimateMeshRoutine(TMP_TextInfo textInfo, TMP_MeshInfo[] cachedMeshInfo, float duration, float[] scales, float[] yOffsets, Color32[] colors, float[] shakeIntensities)
        {
            float timer = 0f;
            int charCount = textInfo.characterCount;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                for (int i = 0; i < charCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int matIndex = charInfo.materialReferenceIndex;
                    int vertIndex = charInfo.vertexIndex;

                    Vector3[] sourceVerts = cachedMeshInfo[matIndex].vertices;
                    Vector3[] destVerts = textInfo.meshInfo[matIndex].vertices;
                    Color32[] destColors = textInfo.meshInfo[matIndex].colors32;
                    
                    Vector3 center = (sourceVerts[vertIndex + 0] + sourceVerts[vertIndex + 2]) / 2f;
                    
                    float glitchX = (shakeIntensities != null && shakeIntensities[i] > 0) ? Random.Range(-shakeIntensities[i], shakeIntensities[i]) : 0f;
                    float glitchY = (shakeIntensities != null && shakeIntensities[i] > 0) ? Random.Range(-shakeIntensities[i], shakeIntensities[i]) : 0f;

                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 vert = sourceVerts[vertIndex + j];
                        
                        if (scales != null) vert = center + (vert - center) * scales[i];
                        
                        vert += new Vector3(glitchX, yOffsets[i] + glitchY, 0);

                        destVerts[vertIndex + j] = vert;
                        destColors[vertIndex + j] = colors[i];
                    }
                }
                
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                    _resultText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return null;
            }
        }
    }
}