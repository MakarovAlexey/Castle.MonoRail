// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps {
	using System;
	using System.Text.RegularExpressions;

	public class ContentSubstitutionStep : IPreCompilationStep
	{

		public static class ExceptionMessages
		{
			public const string ContentPlaceHolderIdAttributeNotFound = "asp:content tag should have 'contentplaceholderid' attribute";
			public const string ContentPlaceHolderIdAttributeEmpty = "asp:content tag 'contentplaceholderid' attribute should have non empty value";
		}
		#region IPreCompilationStep Members

		public void Process(SourceFile file)	{
			file.RenderBody = Internal.RegularExpressions.ContentTag.Replace(
				file.RenderBody,
				delegate(Match match) 
				{
					var parsedAttributes = match.Groups["attributes"].Value;
					var attributes = Utilities.GetAttributesDictionaryFrom(parsedAttributes);
					if (attributes.Contains("runat") && String.Equals("server", (attributes["runat"] as string), StringComparison.InvariantCultureIgnoreCase)) 
					{
						if (!attributes.Contains("contentplaceholderid"))
							throw new AspViewException(ExceptionMessages.ContentPlaceHolderIdAttributeNotFound);
						if (String.IsNullOrEmpty((string)attributes["contentplaceholderid"]))
							throw new AspViewException(ExceptionMessages.ContentPlaceHolderIdAttributeEmpty);
						var contentplaceholderid = (string)attributes["contentplaceholderid"];

						// handle ViewContents special case
						if (contentplaceholderid == "ViewContents")
							return match.Groups["content"].Value;
						var capturefortagformat = @"<component:capturefor id=""{0}"">{1}</component:capturefor>";
						return string.Format(capturefortagformat, contentplaceholderid, match.Groups["content"].Value);
					}
					else
						return match.Value;
				                      
				}
				);
		}

		#endregion
	}
}
