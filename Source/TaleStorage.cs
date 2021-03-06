using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Grammar;
using RimWorld;

namespace RimTales
{
	public class TaleStorage : IExposable
	{
		public string	def;
		public string	customLabel;
		public string	date;

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
			Scribe_Values.Look(ref def, "def");
			Scribe_Values.Look(ref customLabel, "customLabel");
			Scribe_Values.Look(ref date, "date");
		}
	}
}
