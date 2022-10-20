// @Author: tanjinhua
// @Date: 2021/12/20  10:13


using Common;
using WLCore;
using System.Collections.Generic;

namespace MJCommon
{
    public class MJActionEventRecord : BaseRecord
    {
        public struct Data
        {
            public int chairId;
            public int selectShowType;
            public List<int> showTypes;
        }

        private List<Data> _datas;
        private MJActionButtonController _abController;

        public override void OnInitialize()
        {
            _abController = stage.GetController<MJActionButtonController>();
        }

        public override void Read(MsgHeader msg)
        {
            _datas = new List<Data>();
            var count = msg.ReadByte();
            for (int i = 0; i < count; i++)
            {
                var data = new Data
                {
                    chairId = msg.ReadByte(),
                    selectShowType = msg.ReadByte(),
                    showTypes = new List<int>()
                };
                var eventCount = msg.ReadByte();
                for (int j = 0; j < eventCount; j++)
                {
                    data.showTypes.Add(msg.ReadByte());
                }
                _datas.Add(data);
            }
        }

        public override float Execute()
        {
            _abController.ShowReplayButtons(_datas);

            return 0.8f;
        }

        public override void Abort()
        {
            _abController.ClearButtons();
        }

        public override void Undo()
        {
            _abController.ClearButtons();
        }
    }
}
