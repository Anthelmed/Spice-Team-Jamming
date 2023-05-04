using UnityEngine;

namespace SpiceTeamJamming.UI
{
    public class UIFireIceNatureProgression : MonoBehaviour
    {
        [SerializeField] private CustomRenderTexture progressionCustomRenderTexture;
        [SerializeField] Material worldSpaceMat;
        private static readonly int _burntTilesPercentProperty = Shader.PropertyToID("_BurntTilesPercent");
        private static readonly int _frozenTilesPercentProperty = Shader.PropertyToID("_FrozenTilesPercent");
        private static readonly int _natureTilesPercentProperty = Shader.PropertyToID("_NatureTilesPercent");
        float goalBurnt;
        float goalFrozen;

        float currentBurnt;
        float currentFrozen;

        private void OnEnable()
        {
            GameTile.MapTileStatusChanged += OnMapTileStatusChanged;
        }
        
        private void OnDisable()
        {
            GameTile.MapTileStatusChanged -= OnMapTileStatusChanged;
        }
        
        private void OnDestroy()
        {
            SetTilesStatusPercent(0, 0, 1);
        }
    
        private void OnMapTileStatusChanged(MapTileData _)
        {
            UpdateVisualProgression();
        }
        void Update()
        {
            currentBurnt = Mathf.MoveTowards(currentBurnt, goalBurnt, Time.deltaTime * 0.01f);
            currentFrozen = Mathf.MoveTowards(currentFrozen, goalFrozen, Time.deltaTime * 0.01f);
            worldSpaceMat.SetFloat(_burntTilesPercentProperty, currentBurnt);
            worldSpaceMat.SetFloat(_frozenTilesPercentProperty, currentFrozen);
        }
        private void UpdateVisualProgression()
        {
            var totalTilesCount = (float)LevelTilesManager.instance.Tiles.Count;

            var burntTilesCount = GetTilesCountWithStatus(WorldTileStatus.burnt);
            var frozenTilesCount = GetTilesCountWithStatus(WorldTileStatus.frozen);
            var natureTilesCount = GetTilesCountWithStatus(WorldTileStatus.neutral);
    
            var burntTilesPercent = burntTilesCount / totalTilesCount;
            var frozenTilesPercent = frozenTilesCount / totalTilesCount;
            var natureTilesPercent = natureTilesCount / totalTilesCount;

            SetTilesStatusPercent(burntTilesPercent, frozenTilesPercent, natureTilesPercent);
        }

        private void SetTilesStatusPercent(float burntTilesPercent, float frozenTilesPercent, float natureTilesPercent)
        {
            goalBurnt = burntTilesPercent;
            goalFrozen = frozenTilesPercent;
            
            progressionCustomRenderTexture.material.SetFloat(_burntTilesPercentProperty, burntTilesPercent);
            progressionCustomRenderTexture.material.SetFloat(_frozenTilesPercentProperty, frozenTilesPercent);
            progressionCustomRenderTexture.material.SetFloat(_natureTilesPercentProperty, natureTilesPercent);
        }
    
        private int GetTilesCountWithStatus(WorldTileStatus status)
        {
            var count = 0;
    
            foreach (var tile in LevelTilesManager.instance.Tiles)
                if (tile.MapTile.mapTileData.tileStatus == status)
                    count++;
    
            return count;
        }
    }
}
