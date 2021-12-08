using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Grammar;
using RimWorld;

namespace RimTales
{
	public class TaleStorage : IExposable
	{
		public TaleDef  def;
		public string   customLabel;

		public virtual string ShortSummary
		{
			get
			{
				if (!customLabel.NullOrEmpty())
				{
					return customLabel.CapitalizeFirst();
				}
				return "";
			}
		}

		public virtual void ExposeData()
		{
			Scribe_Defs.Look(ref def, "def");
			Scribe_Values.Look(ref customLabel, "customLabel");
		}

		
	}
}
