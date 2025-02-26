using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Board
{
    public class Board : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] GameObject _prefabCard;
        [SerializeField] List<BoardFaction> _objFactions;

        private void Start()
        {
            RefreshCardPosition();
        }

        [Sirenix.OdinInspector.Button]
        void RefreshCardPosition()
        {
            ArrangeFactionCards(_objFactions[0], true); // Bottom faction
            ArrangeFactionCards(_objFactions[1], false); // Top faction
        }

        void ArrangeFactionCards(BoardFaction faction, bool isBottom)
        {
            var positions = faction.GetPositions();
            RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
            float screenHeight = canvasRect.rect.height;
            float screenWidth = canvasRect.rect.width;
            
            float colSpacing = screenWidth / 3; // 3 cột, chia đều theo chiều ngang
            float rowSpacing = screenHeight / 6; // 3 hàng, chia đều theo chiều dọc
            float startX = -screenWidth / 2 + colSpacing / 2; // Căn giữa theo chiều ngang
            float paddingY = screenHeight * 0.1f; // Neo về gần viền màn hình với khoảng cách 10%
            float startY = isBottom ? -screenHeight / 2 + paddingY : screenHeight / 2 - paddingY;

            foreach (var pos in positions)
            {
                if (pos.ObjCard != null)
                {
                    RectTransform cardTransform = pos.ObjCard.GetComponent<RectTransform>();
                    int row = pos.Index / 3;
                    int col = pos.Index % 3;
                    float xPos = startX + (col * colSpacing);
                    float yPos = startY - (row * rowSpacing * (isBottom ? -1 : 1));
                    cardTransform.anchoredPosition = new Vector2(xPos, yPos);
                }
            }
        }
    }
}