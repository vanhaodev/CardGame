namespace World.Board
{
    public class RoundResultModel
    {
        /// <summary>
        /// 1: phe 1 (ở bottom là player) thắng <br/>
        /// 2: phe 2 (top của NPC) thắng <br/>
        /// 3: hòa <br/>
        /// 0: chưa có gì 
        /// </summary>
        public int WinFactionIndex;
        public string Debug;
    }
}