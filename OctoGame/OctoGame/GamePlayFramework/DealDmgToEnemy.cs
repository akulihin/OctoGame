using System;
using System.Threading.Tasks;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.BonusDmgHandling;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;

namespace OctoGame.OctoGame.GamePlayFramework
{
    public sealed class DealDmgToEnemy : IServiceSingleton
    {
        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly Crit _crit;
        private readonly Dodge _dodge;
        private readonly ArmorReduction _armorReduction;
        private readonly MagicReduction _magicReduction;
        private readonly UserAccounts _accounts;
        private readonly UpdateFightPage _updateFightPage;


        public DealDmgToEnemy(Crit crit, Dodge dodge, ArmorReduction armorReduction, MagicReduction magicReduction, UserAccounts accounts, UpdateFightPage updateFightPage)
        {
            _crit = crit;
            _dodge = dodge;
            _armorReduction = armorReduction;
            _magicReduction = magicReduction;
            _accounts = accounts;
            _updateFightPage = updateFightPage;
        }


        public async Task DmgHealthHandeling(int dmgWhere, double dmg, int dmgType, AccountSettings myAccount,
            AccountSettings enemyAccount)
        {
            //TODO implemnt
            // >on hit dmg

            /*
             0 = Regular
             1 = To health
             2 = only to stamina
             */
            // type 0 = physic, 1 = magic

            var onHitDmg  = _magicReduction.ResistHandling(myAccount.MagicalPenetration, enemyAccount.MagicalResistance, myAccount.OnHitDamage);   
            
            if(myAccount.PrecentBonusDmg > 0)
            dmg =  dmg * (1 + myAccount.PrecentBonusDmg);

            if (dmg > 0)
                if (myAccount.IsCrit)
                dmg = _crit.CritHandling(myAccount.Agility_Stats,
                    dmg, myAccount); // check crit


            dmg = _dodge.DodgeHandling(myAccount.Agility_Stats, dmg,
                myAccount, enemyAccount); // check block


            if(dmg > 0)
            dmg = CheckForBlock(dmgType, dmg, myAccount); // check for block, if dmg <= 0 than dont waste block

            // 0 - artmor
            // 1 - magic 
            // 2 - mix (50/50)
            if (dmg > 0) // armor resist handling
                switch (dmgType)
            {
                case 0:
                    dmg = _armorReduction.ArmorHandling(myAccount.PhysicalPenetration, enemyAccount.PhysicalResistance, dmg);
                    break;
                case 1:
                    dmg = _magicReduction.ResistHandling(myAccount.MagicalPenetration, enemyAccount.MagicalResistance, dmg);   
                    break;
                case 3:
                    var part1 = _armorReduction.ArmorHandling(myAccount.PhysicalPenetration, enemyAccount.PhysicalResistance, dmg/2);
                    var part2 = _magicReduction.ResistHandling(myAccount.MagicalPenetration, enemyAccount.MagicalResistance, dmg/2);
                    dmg = part1 + part2;
                    break;
                case 2:
                    //true dmg - no armor reduction.
                    break;
                default:
                    throw new ApplicationException("Damage Type is not in the range (0 - armor, 1 - magic, 2 - 50/50)");
                //TODO implement mix dmg
            }

            if (dmg > 0)
                myAccount.Health += dmg * myAccount.LifeStealPrec; //lifesteal

            if (myAccount.Health > myAccount.MaxHealth) myAccount.Health = myAccount.MaxHealth; // hp cant be more than MAX hp

            var status = 0;
            var userId = myAccount.Id;

            if (dmg > 0)
                dmg += onHitDmg;
            
            switch (dmgWhere)
            {
                case 0 when enemyAccount.Stamina > 0:

                    if (dmgType == 0)
                    {
                        dmg = dmg - enemyAccount.PhysShield;

                        if (dmg < 0)
                            dmg = 0;
                    }

                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);

                    if (enemyAccount.Stamina < 0)
                    {
                        enemyAccount.Health += enemyAccount.Stamina;
                        enemyAccount.Stamina = 0;
                        _accounts.SaveAccounts(userId);
                    }

                    break;
                case 0:

                    if (dmgType == 0)
                    {
                        dmg = dmg - myAccount.PhysShield;

                        if (dmg < 0)
                            dmg = 0;
                    }

                    if (enemyAccount.Stamina <= 0)
                    {
                        enemyAccount.Health -= Math.Ceiling(dmg);
                        _accounts.SaveAccounts(userId);
                    }

                    break;
                case 1:
                    enemyAccount.Health -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);
                    break;
                case 2:
                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    if (enemyAccount.Stamina < 0)
                        enemyAccount.Stamina = 0;
                    _accounts.SaveAccounts(userId);
                    break;
            }

            if (enemyAccount.Health <= 0)
            {
                enemyAccount.Health = 0;
                status = 1;
            }

            _accounts.SaveAccounts(myAccount.Id);
            _accounts.SaveAccounts(enemyAccount.Id);

            await _updateFightPage.UpdateIfWinOrContinue(status, myAccount.Id, myAccount.MessageIdInList);


        }

        public double CheckForBlock(int dmgType, double dmg, AccountSettings myAccount)
        {

            foreach (var shield in myAccount.BlockShield)
            {
                shield.howManyHits--;
                shield.howManyTurn--;
                _accounts.SaveAccounts(myAccount.Id);

                if (shield.howManyHits <= -1 || shield.howManyTurn <= -1)
                    return dmg;
            }

            foreach (var shield in myAccount.BlockShield)
            {
                switch (dmgType)
                {
                    case 0 when shield.shieldType == 0:
                        return 0;
                    case 1 when shield.shieldType == 1:
                        return 0;

                }
            }

            return dmg;
        }


    }
}
