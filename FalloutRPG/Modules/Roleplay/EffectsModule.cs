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
        private const int MAX_EFFECTS = 10;
        private readonly EffectsService _effectsService;

        public EffectsModule(EffectsService effectsService)
        {
            _effectsService = effectsService;
        }

        [Command("list")]
        public async Task<RuntimeResult> ListEffectsAsync()
        {
            var effects = await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id);

            if (effects == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            var message = new StringBuilder();

            for (var i = 0; i < effects.Count; i++)
            {
                message.Append($"{i + 1}: {effects[i].Name}\n");
            }

            var embed = EmbedHelper.BuildBasicEmbed("Command: $effect list", message.ToString());

            await ReplyAsync(Context.User.Mention, embed: embed);
            return GenericResult.FromSilentSuccess();
        }

        [Command("info")]
        public async Task<RuntimeResult> ViewEffectAsync([Remainder]string name)
        {
            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            await ReplyAsync(message: Context.User.Mention, embed: EmbedHelper.BuildBasicEmbed(null, _effectsService.GetEffectInfo(effect)));
            return GenericResult.FromSilentSuccess();
        }

        [Command("create")]
        public async Task<RuntimeResult> CreateEffectAsync([Remainder]string name)
        {
            if (!StringHelper.IsOnlyLetters(name))
                return GenericResult.FromError(Messages.ERR_EFFECT_NOT_ALPHABETICAL);

            if (name.Length > 24)
                return GenericResult.FromError(Messages.ERR_EFFECT_NAME_TOO_LONG);

            if (await _effectsService.CountEffectsAsync(Context.User.Id) >= MAX_EFFECTS)
                return GenericResult.FromError(Messages.ERR_EFFECT_TOO_MANY);

            if (await _effectsService.IsDuplicateName(name))
                return GenericResult.FromError(Messages.ERR_EFFECT_NAME_DUPLICATE);

            await _effectsService.CreateEffectAsync(name, Context.User.Id);

            return GenericResult.FromSuccess(Messages.EFFECT_CREATE_SUCCESS);
        }

        [Command]
        public async Task<RuntimeResult> EditEffectAsync(string name, Statistic stat, int value)
        {
            var effect = (await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id)).Find(x => x.Name.Equals(name));

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

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

            return GenericResult.FromSuccess(Messages.EFFECT_EDIT_SUCCESS);
        }

        [Command("delete")]
        public async Task<RuntimeResult> DeleteEffectAsync([Remainder]string name)
        {
            var effect = (await _effectsService.GetAllOwnedEffectsAsync(Context.User.Id)).Find(x => x.Name.Equals(name));

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            await _effectsService.DeleteEffectAsync(effect);

            return GenericResult.FromSuccess(Messages.EFFECT_DELETE_SUCCESS);
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
        public async Task<RuntimeResult> ShowCharacterEffectsAsync()
        {
            var userInfo = Context.User;
            var character = await _charService.GetCharacterAsync(userInfo.Id);

            if (character == null) return CharacterResult.CharacterNotFound();

            string info = _effectsService.GetCharacterEffects(character);

            await ReplyAsync(userInfo.Mention, embed: EmbedHelper.BuildBasicEmbed($"{character.Name}'s Effects:", info));
            return GenericResult.FromSilentSuccess();
        }

        [Command("apply")]
        public async Task<RuntimeResult> ApplyEffectAsync([Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(Context.User.Id);

            if (character == null) return CharacterResult.CharacterNotFound();

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            if (character.EffectCharacters.Any(x => x.Effect.Equals(effect)))
                return GenericResult.FromError(Messages.ERR_EFFECT_ALREADY_APPLIED);

            character.EffectCharacters.Add(new EffectCharacter { Character = character, Effect = effect });
            await _charService.SaveCharacterAsync(character);

            return GenericResult.FromSuccess(String.Format(Messages.EFFECT_APPLY_SUCCESS, effect.Name, character.Name));
        }

        [Command("unapply")]
        public async Task<RuntimeResult> RemoveEffectAsync([Remainder]string name)
        {
            var character = await _charService.GetCharacterAsync(Context.User.Id);

            if (character == null) return CharacterResult.CharacterNotFound();

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            //if (character.Effects == null) character.Effects = new List<Effect>();
            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            character.EffectCharacters.Remove(character.EffectCharacters.Where(x => x.Effect.Equals(effect)).FirstOrDefault());
            await _charService.SaveCharacterAsync(character);

            return GenericResult.FromSuccess(String.Format(Messages.EFFECT_REMOVE_SUCCESS, effect.Name, character.Name));
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
        public async Task<RuntimeResult> ShowCharacterEffectsAsync(string name)
        {
            var userInfo = Context.User;
            var character = _npcService.FindNpc(name);

            if (character == null) return CharacterResult.NpcNotFound();

            string info = _effectsService.GetCharacterEffects(character);

            await ReplyAsync(userInfo.Mention, embed: EmbedHelper.BuildBasicEmbed($"{character.Name}'s Effects:", info));
            return GenericResult.FromSilentSuccess();
        }

        [Command("apply")]
        public async Task<RuntimeResult> ApplyEffectAsync(string npcName, [Remainder]string name)
        {
            var character = _npcService.FindNpc(npcName);
            if (character == null) return CharacterResult.NpcNotFound();

            var effect = await _effectsService.GetEffectAsync(name);
            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            if (character.EffectCharacters == null)
                character.EffectCharacters = new List<EffectCharacter>();

            if (character.EffectCharacters.Any(x => x.Effect.Equals(effect)))
                return GenericResult.FromError(Messages.ERR_EFFECT_ALREADY_APPLIED);

            character.EffectCharacters.Add(new EffectCharacter { Character = character, Effect = effect });

            return GenericResult.FromSuccess(String.Format(Messages.EFFECT_APPLY_SUCCESS, effect.Name, character.Name));
        }

        [Command("unapply")]
        public async Task<RuntimeResult> RemoveEffectAsync(string npcName, [Remainder]string name)
        {
            var character = _npcService.FindNpc(npcName);

            if (character == null) return CharacterResult.NpcNotFound();

            var effect = await _effectsService.GetEffectAsync(name);

            if (effect == null) return GenericResult.FromError(Messages.ERR_EFFECT_NOT_FOUND);

            //if (character.Effects == null) character.Effects = new List<Effect>();
            if (character.EffectCharacters == null) character.EffectCharacters = new List<EffectCharacter>();

            character.EffectCharacters.Remove(character.EffectCharacters.Where(x => x.Effect.Equals(effect)).FirstOrDefault());

            return GenericResult.FromSuccess(String.Format(Messages.EFFECT_REMOVE_SUCCESS, effect.Name, character.Name));
        }
    }
}
