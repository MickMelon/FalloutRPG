using Discord.Commands;
using FalloutRPG.Constants;
using FalloutRPG.Helpers;
using FalloutRPG.Models;
using FalloutRPG.Models.Effects;
using FalloutRPG.Services.Roleplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalloutRPG.Modules.Roleplay
{
    [Group("effects")]
    [Alias("effect")]
    public class EffectsModule : ModuleBase
    {
        private readonly EffectsService _effectsService;

        public EffectsModule(EffectsService effectsService)
        {
            _effectsService = effectsService;
        }

        [Command("list")]
        public async Task ListEffectsAsync()
        {
            var effects = await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id);

            if (effects == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            var message = new StringBuilder();

            for (var i = 0; i < effects.Count; i++)
            {
                message.Append($"{i + 1}: {effects[i].Name}\n");
            }

            var embed = EmbedHelper.BuildBasicEmbed("Command: $effect list", message.ToString());

            await ReplyAsync(Context.User.Mention, embed: embed);
        }

        [Command("info")]
        public async Task ViewEffectAsync([Remainder]string name)
        {
            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            await ReplyAsync(message: Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed(null, _effectsService.GetEffectInfo(effect)));
        }

        [Command("create")]
        public async Task CreateEffectAsync([Remainder]string name)
        {
            if (!StringHelper.IsOnlyLetters(name))
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_ALPHABETICAL, Context.User.Mention));
                return;
            }

            if (name.Length > 24)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NAME_TOO_LONG, Context.User.Mention));
                return;
            }

            if (await _effectsService.CountEffectsAsync(Context.User.Id) >= 10)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_TOO_MANY, Context.User.Mention));
                return;
            }

            if (await _effectsService.IsDuplicateName(name))
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NAME_DUPLICATE, Context.User.Mention));
                return;
            }

            await _effectsService.CreateEffectAsync(name, Context.User.Id);

            await ReplyAsync(String.Format(Messages.EFFECT_CREATE_SUCCESS, Context.User.Mention));
        }

        [Command]
        public async Task EditEffectAsync(string name, Statistic stat, int value)
        {
            var effect = (await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id)).Find(x => x.Name.Equals(name));

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            var match = effect.StatisticEffects.FirstOrDefault(x => x.Statistic.Equals(stat));

            if (match != null)
            {
                if (value == 0)
                    effect.StatisticEffects.Remove(match);
                else
                    match.Value = value;
            }
            else
            {
                effect.StatisticEffects.Add(new StatisticValue { Statistic = stat, Value = value });
            }

            await _effectsService.SaveEffectAsync(effect);

            await ReplyAsync(String.Format(Messages.EFFECT_EDIT_SUCCESS, Context.User.Mention));
        }

        [Command("delete")]
        public async Task DeleteEffectAsync([Remainder]string name)
        {
            var effect = (await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id)).Find(x => x.Name.Equals(name));

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            await _effectsService.DeleteEffectAsync(effect);

            await ReplyAsync(String.Format(Messages.EFFECT_DELETE_SUCCESS, Context.User.Mention));
        }
    }

    [Group("character")]
    [Alias("char")]
    public class CharacterEffectsModule : ModuleBase
    {
        private readonly CharacterService _charService;
        private readonly EffectsService _effectsService;

        public CharacterEffectsModule(CharacterService charService, EffectsService effectsService)
        {
            _charService = charService;
            _effectsService = effectsService;
        }

        [Command("effects")]
        [Alias("effect", "wounds", "wound", "buffs", "buff", "debuffs", "debuff")]
        public async Task ShowCharacterEffectsAsync()
        {
            var userInfo = Context.User;
            var character = await _charService.GetCharacterAsync(userInfo.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, userInfo.Mention));
                return;
            }

            string info = _effectsService.GetCharacterEffects(character);

            await ReplyAsync(userInfo.Mention, embed: EmbedHelper.BuildBasicEmbed($"{character.Name}'s Effects:", info));
        }

        [Command("apply")]
        public async Task ApplyEffectAsync([Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(Context.User.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            if (character.EffectCharacters.Any(x => x.Effect.Equals(effect)))
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_ALREADY_APPLIED, Context.User.Mention));
                return;
            }

            character.EffectCharacters.Add(new EffectCharacter { Character = character, Effect = effect });
            await _charService.SaveCharacterAsync(character);

            await ReplyAsync(String.Format(Messages.EFFECT_APPLY_SUCCESS, effect.Name, character.Name, Context.User.Mention));
        }

        [Command("unapply")]
        public async Task RemoveEffectAsync([Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(Context.User.Id);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            //if (character.Effects == null) character.Effects = new List<Effect>();
            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            character.EffectCharacters.Remove(character.EffectCharacters.Where(x => x.Effect.Equals(effect)).FirstOrDefault());
            await _charService.SaveCharacterAsync(character);

            await ReplyAsync(String.Format(Messages.EFFECT_REMOVE_SUCCESS, effect.Name, character.Name, Context.User.Mention));
        }
    }

    [Group("npc")]
    public class NpcEffectsModule : ModuleBase
    {
        private readonly EffectsService _effectsService;
        private readonly NpcService _npcService;

        public NpcEffectsModule(EffectsService effectsService, NpcService npcService)
        {
            _effectsService = effectsService;
            _npcService = npcService;
        }

        [Command("effects")]
        [Alias("effect", "wounds", "wound", "buffs", "buff", "debuffs", "debuff")]
        public async Task ShowCharacterEffectsAsync(string name)
        {
            var userInfo = Context.User;
            var character = _npcService.FindNpc(name);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, userInfo.Mention));
                return;
            }

            string info = _effectsService.GetCharacterEffects(character);

            await ReplyAsync(userInfo.Mention, embed: EmbedHelper.BuildBasicEmbed($"{character.Name}'s Effects:", info));
        }

        [Command("apply")]
        public async Task ApplyEffectAsync(string npcName, [Remainder]string name)
        {
            var character = _npcService.FindNpc(npcName);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            if (character.EffectCharacters.Any(x => x.Effect.Equals(effect)))
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_ALREADY_APPLIED, Context.User.Mention));
                return;
            }

            character.EffectCharacters.Add(new EffectCharacter { Character = character, Effect = effect });

            await ReplyAsync(String.Format(Messages.EFFECT_APPLY_SUCCESS, effect.Name, character.Name, Context.User.Mention));
        }

        [Command("unapply")]
        public async Task RemoveEffectAsync(string npcName, [Remainder]string name)
        {
            var character = _npcService.FindNpc(npcName);

            if (character == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_NPC_CHAR_NOT_FOUND, Context.User.Mention));
                return;
            }

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null)
            {
                await ReplyAsync(String.Format(Messages.ERR_EFFECT_NOT_FOUND, Context.User.Mention));
                return;
            }

            //if (character.Effects == null) character.Effects = new List<Effect>();
            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            character.EffectCharacters.Remove(character.EffectCharacters.Where(x => x.Effect.Equals(effect)).FirstOrDefault());

            await ReplyAsync(String.Format(Messages.EFFECT_REMOVE_SUCCESS, effect.Name, character.Name, Context.User.Mention));
        }
    }
}
