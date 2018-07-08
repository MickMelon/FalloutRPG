﻿using Discord;
using Discord.WebSocket;
using FalloutRPG.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FalloutRPG.Services
{
    public class CrapsService
    {
        private readonly GamblingService _gamblingService;

        private readonly Random _rand;

        private List<IUser> _players;
        private ISocketMessageChannel _channel;
        private List<Bet> _bets;

        public IUser Shooter { get; private set; }
        private const int SHOOTER_TIMEOUT_SECONDS = 45;
        private int _shooterIndex;

        private Round _round;
        private int _shooterPoint;

        private Timer _rollTimer;

        public CrapsService(GamblingService gamblingService)
        {
            _players = new List<IUser>();
            _bets = new List<Bet>();

            _round = Round.ComeOut;
            _shooterPoint = -1;

            _shooterIndex = 0;

            _gamblingService = gamblingService;

            _rand = new Random();

            _rollTimer = new Timer(SHOOTER_TIMEOUT_SECONDS * 1000);
            _rollTimer.Elapsed += ShooterTimeout;
        }

        private async void ShooterTimeout(object sender, ElapsedEventArgs e)
        {
            // if shooter does not have a bet, skip them
            // if they do, roll for them
            
            if (HasBet(Shooter)) // shooter bet exists
            {
                await _channel.SendMessageAsync(String.Format(Messages.CRAPS_INACTIVITY_ROLL, Shooter.Mention));
                await _channel.SendMessageAsync(Roll(Shooter));
            }
            else
            {
                if (_players.Count == 1)
                {
                    await _channel.SendMessageAsync(String.Format(Messages.CRAPS_INACTIVITY_KICK, Shooter.Mention));
                    LeaveMatch(Shooter);
                    _rollTimer.Stop();
                }
                else
                {
                    var oldShooterMention = Shooter.Mention;
                    NextShooter();
                    await _channel.SendMessageAsync(String.Format(Messages.CRAPS_INACTIVITY_PASS_DICE, Shooter.Mention, oldShooterMention));
                }
            }
        }

        public string Roll(IUser user)
        {
            if (_players.Count == 0)
                return String.Format(Messages.CRAPS_EMPTY_MATCH, user.Mention);
            if (user != Shooter)
                return String.Format(Messages.ERR_CRAPS_NOT_SHOOTER, user.Mention, Shooter.Mention);
            if (_bets.Find(x => x.User.Equals(user)) == null)
                return String.Format(Messages.ERR_CRAPS_BET_NOT_SET, user.Mention);

            string result = "";

            int dice1 = _rand.Next(1, 7),
                dice2 = _rand.Next(1, 7),
                sum = dice1 + dice2;

            result = DiceToEmoji(dice1) + " " + DiceToEmoji(dice2) + "\n";
            _rollTimer.Stop();
            _rollTimer.Start();
            return result + AdvanceRound(sum);
        }

        public string PlaceBet(IUser user, string betToPlace, int betAmount)
        {
            if (_bets.Find(x => x.User.Equals(user)) != null) // bet already placed
                return String.Format(Messages.ERR_CRAPS_BET_ALREADY_SET, user.Mention);

            if (!_gamblingService.UserBalances.ContainsKey(user)) // can't find user in dictionary
                if (_gamblingService.AddUserBalanceAsync(user).Result != GamblingService.AddUserBalanceResult.Success) // failed to add user
                    return String.Format(Messages.ERR_BALANCE_ADD_FAIL, user.Mention);

            if (betAmount > _gamblingService.UserBalances[user])
                return String.Format(Messages.ERR_BET_TOO_HIGH, user.Mention);

            if (betAmount <= 0)
                return String.Format(Messages.ERR_BET_TOO_LOW, user.Mention);

            betToPlace = betToPlace.ToLower();

            BetType betType = BetType.Error;

            switch (betToPlace)
            {
                case "pass":
                    betType = BetType.Pass;
                    break;
                case "dontpass":
                    betType = BetType.DontPass;
                    break;
                case "come":
                    betType = BetType.Come;
                    break;
                case "dontcome":
                    betType = BetType.DontCome;
                    break;
            }

            if (betType != BetType.Error)
            {
                if (_round == Round.ComeOut)
                {
                    // can't place a Come bet during the come out round
                    if (betType == BetType.Come || betType == BetType.DontCome)
                        return String.Format(Messages.ERR_CRAPS_POINT_NOT_SET, user.Mention);
                }
                else if (_round == Round.Point)
                {
                    // can't place a Pass bet during the point round
                    if (betType == BetType.Pass || betType == BetType.DontPass)
                        return String.Format(Messages.ERR_CRAPS_POINT_ALREADY_SET, user.Mention);
                }

                _bets.Add(new Bet(user, betAmount, betType));
                if (user == Shooter)
                {
                    _rollTimer.Stop();
                    _rollTimer.Start();
                }
                return String.Format(Messages.BET_PLACED, user.Mention);
            }
            else
            {
                // failed to parse bet type
                return String.Format(Messages.ERR_CRAPS_BET_PARSE_FAIL, user.Mention);
            }
        }

        private string AdvanceRound(int roll)
        {
            string result = "";

            result += AwardPassBets(roll);
            result += AwardComeBets(roll);
            return result;
        }

        public async Task<JoinMatchResult> JoinMatch(IUser user, ISocketMessageChannel channel)
        {
            _channel = channel;

            var result = await _gamblingService.AddUserBalanceAsync(user);

            if (result == GamblingService.AddUserBalanceResult.Success || result == GamblingService.AddUserBalanceResult.AlreadyInDictionary)
            {
                if (!_players.Contains(user))
                    _players.Add(user);
                else
                    return JoinMatchResult.AlreadyInMatch;

                if (_players.Count == 1)
                {
                    Shooter = user;
                    _rollTimer.Start();
                    return JoinMatchResult.NewMatch;
                }
                return JoinMatchResult.Success;
            }
            else if (result == GamblingService.AddUserBalanceResult.NullCharacter)
                return JoinMatchResult.NullCharacter;
            else
                return JoinMatchResult.UnknownError;
        }

        public enum JoinMatchResult
        {
            Success,
            NewMatch,
            AlreadyInMatch,
            NullCharacter,
            UnknownError
        }

        public bool LeaveMatch(IUser user)
        {
            if (user == Shooter && HasBet(user))
                return false;

            _players.Remove(user);

            return true;
        }

        private bool HasBet(IUser user)
        {
            if (_bets.Find(x => x.User == user) == null)
                return false;
            return true;
        }

        /// <summary>
        /// Changes the active shooter to the next player in the list.
        /// </summary>
        /// <returns>True if the shooter changed, False if the new shooter is the same as the previous.</returns>
        public bool NextShooter()
        {
            IUser oldShooter = Shooter;

            if (HasBet(Shooter))
                return false;

            if (_players.Count > _shooterIndex + 1) // players remaining in list
            {
                Shooter = _players[++_shooterIndex];
            }
            else
            {
                _shooterIndex = 0; // set _shooterIndex to 0 to loop back around the "table"
                Shooter = _players[_shooterIndex];
            }

            // give the new shooter some time to roll :)
            _rollTimer.Stop();
            _rollTimer.Start();

            if (Shooter != oldShooter)
                return true;
            else
                return false;
        }

        private string AwardPassBets(int roll)
        {
            string result = "";

            BetType winningBet = BetType.Error;

            if (_round == Round.ComeOut)
            {
                if (roll == 2 || roll == 3 || roll == 12) // crap out
                {
                    winningBet = BetType.DontPass;
                    result += String.Format(Messages.CRAPS_CRAPOUT, Shooter.Mention) + "\n";
                }
                else if (roll == 7 || roll == 11) // natural
                {
                    winningBet = BetType.Pass;
                    result += String.Format(Messages.CRAPS_NATURAL, Shooter.Mention) + "\n";
                }
                else // point
                {
                    _shooterPoint = roll;
                    _round = Round.Point;
                    result += String.Format(Messages.CRAPS_POINT_ROUND, Shooter.Mention) + "\n";
                }
            }
            else if (_round == Round.Point)
            {
                if (roll == 7) // seven out
                {
                    winningBet = BetType.DontPass;

                    result += String.Format(Messages.CRAPS_SEVEN_OUT, Shooter.Mention) + "\n";
                    _round = Round.ComeOut;
                    NextShooter();
                    result += String.Format(Messages.CRAPS_NEW_SHOOTER, Shooter.Mention, roll) + "\n";
                }
                else if (roll == _shooterPoint) // shooter reached point
                {
                    winningBet = BetType.Pass;

                    _round = Round.ComeOut;
                    result += String.Format(Messages.CRAPS_POINT_ROLL, Shooter.Mention) + "\n";
                }
            }
            BetType losingBet = BetType.Error;

            if (winningBet == BetType.Pass)
                losingBet = BetType.DontPass;
            else if (winningBet == BetType.DontPass)
                losingBet = BetType.Pass;

            foreach (var bet in _bets.ToList())
            {
                if (bet.BetType == winningBet)
                    AwardBet(bet);
                else if (bet.BetType == losingBet)
                    AwardBet(bet, false);
            }
            return result;
        }

        private string AwardComeBets(int roll)
        {
            string result = "";

            foreach (var bet in _bets.ToList())
            {
                if (bet.BetType == BetType.Come)
                {
                    if (roll == bet.Point) // come point achieved (win bet)
                    {
                        AwardBet(bet);
                        result += String.Format(Messages.CRAPS_POINT_ROLL, bet.User.Mention) + "\n";
                    }
                    else if (bet.Point == -1) // "come out" roll for new Come Bet
                    {
                        if (roll == 7 || roll == 11) // natural
                        {
                            AwardBet(bet);
                            result += String.Format(Messages.CRAPS_NATURAL, bet.User.Mention) + "\n";
                        }
                        else if (roll == 2 || roll == 3 || roll == 12) // crap out
                        {
                            AwardBet(bet, false);
                            result += String.Format(Messages.CRAPS_CRAPOUT, bet.User.Mention) + "\n";
                        }
                        else // point
                        {
                            bet.Point = roll;
                            result += String.Format(Messages.CRAPS_POINT_SET, bet.User.Mention, roll) + "\n";
                        }
                    }
                }
                else if (bet.BetType == BetType.DontCome)
                {
                    if (roll == bet.Point) // point achieved
                    {
                        AwardBet(bet, false);
                        result += String.Format(Messages.CRAPS_POINT_ROLL_NEG, bet.User.Mention, roll) + "\n";
                    }
                    else if (bet.Point != -1 && roll == 7) // seven out
                    {
                        AwardBet(bet);
                        result += String.Format(Messages.CRAPS_SEVEN_OUT_POS, bet.User.Mention, roll) + "\n";
                    }
                    else if (bet.Point == -1) // come out roll
                    {
                        if (roll == 2 || roll == 3) // crap out
                        {
                            AwardBet(bet);
                            result += String.Format(Messages.CRAPS_CRAPOUT_POS, bet.User.Mention, roll) + "\n";
                        }
                        else if (roll == 12) // crap out push (house always wins)
                        {
                            AwardBet(bet, false, true);
                            result += String.Format(Messages.CRAPS_CRAPOUT_PUSH, bet.User.Mention, roll) + "\n";
                        }
                        else if (roll == 7 || roll == 11) // natural
                        {
                            AwardBet(bet, false);
                            result += String.Format(Messages.CRAPS_NATURAL_NEG, bet.User.Mention, roll) + "\n";
                        }
                        else
                        {
                            bet.Point = roll; // point
                            result += String.Format(Messages.CRAPS_POINT_SET, bet.User.Mention, roll) + "\n";
                        }
                    }
                }
            }

            return result;
        }

        private void AwardBet(Bet bet, bool win = true, bool push = false)
        {
            var balances = _gamblingService.UserBalances;
            if (win)
            {
                balances[bet.User] += bet.BetAmount;
                _bets.Remove(bet);
            }
            else
            {
                balances[bet.User] -= bet.BetAmount;
                _bets.Remove(bet);
            }
        }

        private string DiceToEmoji(int num)
        {
            switch (num)
            {
                case 1:
                    return ":one:";
                case 2:
                    return ":two:";
                case 3:
                    return ":three:";
                case 4:
                    return ":four:";
                case 5:
                    return ":five:";
                case 6:
                    return ":six:";
            }
            return null;
        }

        private enum Round
        {
            ComeOut,
            Point
        }

        private enum RoundResult
        {
            CrapOut,
            Natural,
            SevenOut
        }

        public enum BetType
        {
            Error,
            Pass,
            DontPass,
            Come,
            DontCome
        }

        private class Bet
        {
            public IUser User { get; }
            public int BetAmount { get; }
            public BetType BetType { get; }
            public int Point { get; set; }

            public Bet(IUser user, int betAmount, BetType betType)
            {
                User = user;
                BetAmount = betAmount;
                BetType = betType;
                Point = -1;
            }
        }
    }
}
