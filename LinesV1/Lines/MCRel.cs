using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lines
{
    public class mcCellRelations
    {
        public int SN = -1;
        public mcCell cell = null;

        public mcCell nR = null;
        int rSn = -1;
        public mcCell nL = null;
        int lSn = -1;
        public mcCell nT = null;
        int tSn = -1;
        public mcCell nD = null;
        int dSn = -1;
        public mcCell nTR = null;
        int trSn = -1;
        public mcCell nTL = null;
        int tlSn = -1;
        public mcCell nDR = null;
        int drSn = -1;
        public mcCell nDL = null;
        int dlSn = -1;
        public void SetNeigboursSN()
        {
            if (CanGo(Directions.Right)){ rSn = nR._celRel.SN; }
            if (CanGo(Directions.Left)) { lSn = nL._celRel.SN; }
            if (CanGo(Directions.Top)) { tSn = nT._celRel.SN; }
            if (CanGo(Directions.Down)) { dSn = nD._celRel.SN; }
            if (CanGo(Directions.RightTop)) { trSn = nTR._celRel.SN; }
            if (CanGo(Directions.RightDown)) { drSn = nDR._celRel.SN; }
            if (CanGo(Directions.LeftTop)) { tlSn = nTL._celRel.SN; }
            if (CanGo(Directions.LeftDown)) { dlSn = nDL._celRel.SN; }
        }
        public MCMatrixLines MB = null;

        public bool IsBorderCell()
        {
            if (cell._cellPosBord != mcPositionBorder.C)
                return true;
            return false;
        }
        public bool CanGo(Directions where)
        {
            switch (where)
            {
                case Directions.Right:
                    if (nR != null || rSn!=-1) return true;
                    break;
                case Directions.Left:
                    if (nL != null || lSn!=-1) return true;
                    break;
                case Directions.Top:
                    if (nT != null || tSn!=-1) return true;
                    break;
                case Directions.Down:
                    if (nD != null || dSn!=-1) return true;
                    break;
                case Directions.RightTop:
                    if (nTR != null || trSn!=-1) return true;
                    break;
                case Directions.LeftTop:
                    if (nTL != null || tlSn!=-1) return true;
                    break;
                case Directions.RightDown:
                    if (nDR != null || drSn!=-1) return true;
                    break;
                case Directions.LeftDown:
                    if (nDL != null || dlSn!=-1) return true;
                    break;
                default:
                    break;
            }
            return false;
        }
        public bool IsOcсupied()
        {
            int i = 0;
            if (nR != null) { if (nR._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nL != null) { if (nL._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nT != null) { if (nT._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nD != null) { if (nD._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nTR != null) { if (nTR._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nTL != null) { if (nTL._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nDR != null) { if (nDR._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (nDL != null) { if (nDL._cellBlock == mcCellBlock.Yes) i++; }
            else { i++; }
            if (i == 8)
                return true;
            return false;
        }
        public bool IsMyNeighbour(mcCellRelations obj) 
        {
            if (obj != null)
            {
                int sn = obj.SN;
                int i = 0;

                if (
                    rSn == sn ||
                    lSn == sn ||
                    tSn == sn ||
                    dSn == sn ||
                    trSn == sn ||
                    tlSn == sn ||
                    drSn == sn ||
                    dlSn == sn)
                    return true;
            }
            return false;
        }
        public bool IsAllIdentity(List<mcCellRelations> mcs, mcCellRelations mcCell) 
        {
            if (mcs != null && mcCell != null) 
            {
                foreach (var item in mcs)
                {
                    if (item.SN != mcCell.SN) return false;
                }
            }
            return true;
        }
        public mcCellRelations() {}
        public mcCellRelations(mcCellRelations obj)
        {
            nR = obj.nR;
            nL = obj.nL;
            nT = obj.nT;
            nD = obj.nD;
            nTR = obj.nTR;
            nTL = obj.nTL;
            nDR = obj.nDR;
            nDL = obj.nDL;
            MB = obj.MB;
        }
    }
}
