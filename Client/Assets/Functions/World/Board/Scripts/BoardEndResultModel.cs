namespace World.Board
{
    public class BoardEndResultModel
    {
        public bool IsEnd;
        /// <summary>
        /// 1: phe 1 (ở bottom là player) thắng <br/>
        /// 2: phe 2 (top của NPC) thắng <br/>
        /// 0: hòa
        /// </summary>
        public int WinFactionIndex;
        public string Debug;
    }
}