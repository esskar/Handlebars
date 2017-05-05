﻿using System;

namespace Handlebars
{
	public class DescriptionAttribute : Attribute
	{
		public string Description { get; set; }

		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}