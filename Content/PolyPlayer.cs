using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Audio;

namespace Polyphemalus;

public class PolyPlayer : ModPlayer
{
    public int ChainSawCharge;
    public int ChainSawHitCooldown = 0;
    public int ChainSawLevel1 = 1;
    public int ChainSawLevel2 = 30 * 5;
    public int ChainSawLevel3 = 30 * 10;
    public int ChainSawChargeCritMax = 30 * 15;
    public int ChainSawChargeMax = 30 * 20;

    public bool PolyShieldChargeEnabled;
    public int PolyShieldCooldown;
    public bool PolyShieldChargeKeypress;
    public int PolyShieldChargeDuration;
    public int PolyShieldChargeSpeed = 16;
    public int PolyShieldChargeDurationMax = 25;
    public int DashBuffer = 5;
    public int DashBufferEarly = 5;
    public int DashBufferLate = 5;
    public Vector2 PolyShieldChargeDir = new Vector2(0,0);
    private List<NPC> alreadyRicochet = new List<NPC>();
    public override void ResetEffects()
    {
        PolyShieldChargeEnabled = false;
    }

    public override void PreUpdate()
    {
        if (ChainSawCharge >= ChainSawLevel1)
        {
            Player.ClearBuff(BuffID.Darkness);
            Player.AddBuff(BuffID.Darkness, ChainSawCharge);
        }
        if (ChainSawCharge >= ChainSawLevel2)
        {
            Player.ClearBuff(BuffID.Blackout);
            Player.AddBuff(BuffID.Blackout, ChainSawCharge - (30 * 4));
        }
        if (ChainSawCharge >= ChainSawLevel3)
        {
            Player.ClearBuff(BuffID.Obstructed);
            Player.AddBuff(BuffID.Obstructed, ChainSawCharge - (30 * 10));
        }

        ChainSawHitCooldown--;
        if (ChainSawHitCooldown < 0)
        {
            ChainSawCharge--;
            if (ChainSawCharge < 0)
            {
                ChainSawCharge = 0;
            }
            ChainSawHitCooldown = 0;
        }
        

    }

    public override void PreUpdateMovement()
    {
        if (PolyKeybinds.PolyDashKeybind.JustPressed)
        {
            DashBuffer = 6;
            DashBufferEarly = DashBuffer+5;
            if (DashBufferLate > 0)
            {
                SoundEngine.PlaySound(SoundID.Item50);
                DashBufferEarly = 0;
                DashBufferLate = 0;
            }
        }
        if (DashBuffer > 0 && PolyShieldChargeEnabled && Player.dashDelay == 0 && PolyShieldCooldown == 0 && !Player.mount.Active && !Player.pulley && Player.grappling[0] == -1 && !Player.tongued)
        {
            this.PolyShieldChargeKeypress = true;

        }
        if (PolyShieldChargeKeypress)
        {
            DashBuffer = 0;
            DashBufferEarly = 0;
            PolyShieldChargeDuration = PolyShieldChargeDurationMax;
            PolyShieldChargeKeypress = false;
            PolyShieldChargeDir = DashDirectionCheck();
            Player.timeSinceLastDashStarted = 0;
            base.Player.dashDelay = -1;
            PolyShieldCooldown = -1;
        }
        if (PolyShieldChargeDuration > 0)
        {
            PolyDashCollisionCheck();
            Player.maxFallSpeed = 50f;
            Player.gravity = 0;
            Player.controlUp = false;
            Player.velocity = PolyShieldChargeDir * PolyShieldChargeSpeed;
            Player.armorEffectDrawShadow = true;
            PolyShieldChargeDuration--;

            if (PolyShieldChargeDuration <= 0)
            {
                PolyShieldCooldown = 60;
                base.Player.dashDelay = 30;
                Player.velocity /= 2;
                alreadyRicochet.Clear();
            }
        }
        if (PolyShieldCooldown > 0)
        {
            PolyShieldCooldown--;
            if (PolyShieldCooldown == 0)
            {
                SoundEngine.PlaySound(SoundID.MaxMana);
            }
        }
        if (DashBuffer > 0) { DashBuffer--; }
        if (DashBufferEarly > 0) { DashBufferEarly--; }
        if (DashBufferLate > 0) { DashBufferLate--; }
    }
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (PolyShieldChargeDuration > 0 && Player.whoAmI == Main.myPlayer)
        {
            drawInfo.cShield = 1;
            r = 1;
            b = 1f;
            g = 1;
        } else
        if (PolyShieldCooldown > 0)
        {
            r = 0.8f;
            b = 0.8f;
            g = 0.8f;
        }
    }

    public Vector2 DashDirectionCheck()
    {
        Vector2 x = new Vector2(0, 0);
        if (Player.controlLeft == true) x.X -= 1;
        if (Player.controlRight == true) x.X += 1;
        if (Player.controlUp == true) x.Y -= 1;
        if (Player.controlDown == true) x.Y += 1;
        if (x == new Vector2(0, 0))
        {
            x.X = Player.direction;
        }
        x = x.SafeNormalize(Vector2.One);
        return x;
    }
    public bool PolyDashCollisionCheck()
    {
        Rectangle hitArea = new Rectangle((int)((double)base.Player.position.X + (double)base.Player.velocity.X * 0.75 - 6.0), (int)((double)base.Player.position.Y + (double)base.Player.velocity.Y * 0.75 - 6.0), base.Player.width + 12, base.Player.height + 12);
        for (int i = 0; i < 200; i++)
        {
            NPC npc = Main.npc[i];
            if ((base.Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[npc.type]) || !npc.active || npc.dontTakeDamage || npc.friendly || !hitArea.Intersects(npc.getRect()) || (!npc.noTileCollide && !base.Player.CanHit(npc)))
            {
                continue;
            }
            if (alreadyRicochet.Contains(npc))
            {
                Player.SetImmuneTimeForAllTypes(12);
            } else { 
            Player.ApplyDamageToNPC(npc, (int)(Player.GetDamage(DamageClass.Generic).ApplyTo(80f * (1+alreadyRicochet.Count()*0.5f))), 10, Player.direction, false);
                Player.SetImmuneTimeForAllTypes(12);
                PolyShieldChargeDir *= -1;
            Player.velocity *= -1;
            alreadyRicochet.Add(npc);
            if (DashBuffer > 0)
                {
                    PolyShieldChargeDir=DashDirectionCheck();
                    PolyShieldChargeDuration = PolyShieldChargeDurationMax;
                    SoundEngine.PlaySound(SoundID.Item26);
                    DashBufferEarly=0;
                    DashBufferLate=0;
                } else if (DashBufferEarly> 0)
                {
                    SoundEngine.PlaySound(SoundID.Item49);
                }
                else
                {
                    DashBufferLate = 5;
                }
                return true;
            }
        }
        return false;
    }
}