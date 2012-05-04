using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InsipidusEngine.Battle
{
    class BattleInfo
    {
        #region Fields
        private BattleType _BattleType;
        private List<Party> _ParticipatingParties;
        #endregion

        #region Poperties
        public BattleType BattleType
        {
            get { return _BattleType; }
            set { _BattleType = value; }
        }

        public List<Party> ParticipatingParties
        {
            get { return _ParticipatingParties; }
            set { _ParticipatingParties = value; }
        }
        #endregion

        #region Constructors
        public BattleInfo(BattleType battleType, Party party1, Party party2)
        {
            _BattleType = battleType;
            _ParticipatingParties = new List<Party>();
            _ParticipatingParties.Add(party1);
            _ParticipatingParties.Add(party2);
        }
        #endregion

        #region Methods

        #endregion
    }
}
