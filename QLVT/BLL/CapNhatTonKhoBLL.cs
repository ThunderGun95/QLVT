using QLVT.DAL;
using QLVT.Models;

namespace QLVT.BLL
{
    public class CapNhatTonKhoBLL
    {
        private readonly CapNhatTonKhoDAL capNhatTonKhoDAL;

        public CapNhatTonKhoBLL()
        {
            capNhatTonKhoDAL = new CapNhatTonKhoDAL();
        }

        public List<TonKhoRebuildItem> GetChenhLechTonKho(bool includeZeroInventoryWithoutTransactions)
        {
            return capNhatTonKhoDAL.GetChenhLechTonKho(includeZeroInventoryWithoutTransactions);
        }

        public TonKhoRebuildResult RebuildTonKho(bool zeroInventoryWithoutTransactions)
        {
            return capNhatTonKhoDAL.RebuildTonKho(zeroInventoryWithoutTransactions);
        }
    }
}
