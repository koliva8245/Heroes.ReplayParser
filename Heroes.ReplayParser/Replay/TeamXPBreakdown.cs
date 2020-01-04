﻿using System;

namespace Heroes.ReplayParser.Replay
{
    /// <summary>
    /// Contains the properties for the periodic experience breakdown.
    /// </summary>
    public class TeamXPBreakdown
    {
        /// <summary>
        /// Gets or sets the level of the team.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the time of this current event.
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Gets or sets the current experience earned from minions.
        /// </summary>
        public int MinionXP { get; set; }

        /// <summary>
        /// Gets or sets the current experience earned from mercenaries.
        /// </summary>
        public int MercenaryXP { get; set; }

        /// <summary>
        /// Gets or sets the current experience earned from structures.
        /// </summary>
        public int StructureXP { get; set; }

        /// <summary>
        /// Gets or sets the current experience earned from heroes.
        /// </summary>
        public int HeroXP { get; set; }

        /// <summary>
        /// Gets or sets the current experience earned from the passive experience gain.
        /// </summary>
        public int PassiveXP { get; set; }

        /// <summary>
        /// Gets the total experience earned.
        /// </summary>
        public long TotalXP => MinionXP + MercenaryXP + StructureXP + HeroXP + PassiveXP;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Level: {Level} - {Time} - {TotalXP}";
        }
    }
}
