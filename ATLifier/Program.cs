using System;
using System.Collections.Generic;

using folderStuffs;

namespace ATLifier
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set up our file io
            FolderReview file = new FolderReview();

            //Data from sprite chart
            List<string> sprite_chart_old = file.read("sprite-chart.rpy","Old");

            //The new sprite chart
            List<string> sprite_chart_new = new List<string>();

            //Existing sprite codes
            List<string> existing_sprite_codes = new List<string>();

            //The data we want:
            List<List<string>> sprite_definitions_old = new List<List<string>>();

            //Renamed old sprite defs
            List<string> sprite_definitions_old_renamed = new List<string>();

            //The new sprite defs
            List<string> sprite_definitions_new = new List<string>();

            //ATL sprite chart
            List<string> sprite_chart_two = new List<string>();

            //List of sprites we're adding that need to be aliased
            List<string> codes_to_alias = new List<string>();

            int line_count = 0;
            bool done = false;
            //Now we go thru the data
            //First, we get rid of the 4k lines of code
            foreach (string line in sprite_chart_old)
            {
                if (line.Contains("image monika") && line.Contains("DynamicDisplayable"))
                    for (int i=sprite_chart_old.LastIndexOf(line);i<sprite_chart_old.LastIndexOf(line)+7;i++)
                        if (sprite_chart_old[i].Contains("eyes="))
                            if (!(sprite_chart_old[i].Contains("closedsad") || sprite_chart_old[i].Contains("closedhappy") || sprite_chart_old[i].Contains("wink")))
                                    done = true;

                if (done)
                    break;

                sprite_chart_new.Add(line);
                line_count++;
            }
            sprite_chart_old.RemoveRange(0,line_count);

            //Now we iter and get all the sprite defs as string lists
            List<string> sprite = new List<string>();

            foreach (string line in sprite_chart_old)
            {
                if (line.Contains("image monika") && line.Contains("DynamicDisplayable"))
                {
                    //Don't add this if it's not needed
                    if (sprite.Count != 0)
                        sprite_definitions_old.Add(sprite);

                    //Reset this
                    sprite = new List<string>();
                }

                //This is all excess stuff that we want, but not for amending sprites
                if (line.Contains("### [IMG032]"))
                    break;

                sprite.Add(line);
            }

            //Add the last sprite
            sprite_definitions_old.Add(sprite);

            //Get a list of all defined sprites
            foreach (List<string> strL in sprite_definitions_old)
            {
                existing_sprite_codes.Add(GetSpriteCode(strL[0]));
            }

            //Get rid of all the sprites so all that's left is the extra defs/ATLs
            foreach (List<string> strL in sprite_definitions_old)
            {
                foreach (string s in strL)
                {
                    sprite_chart_old.Remove(s);
                }
            }

            //Now we create and add our new sprite objects
            int count = 0;
            foreach (List<string> strL in sprite_definitions_old)
            {
                MonikaSprite spr = GetSpriteFromCode(existing_sprite_codes[count]);
                spr.eyes = "closedsad";

                if (!existing_sprite_codes.Contains(GetSpriteCodeFromObject(spr)))
                {
                    codes_to_alias.Add(GetSpriteCodeFromObject(spr));
                    sprite_definitions_old_renamed.AddRange(BuildOldSprite(spr));
                }

                sprite_definitions_new.AddRange(CreateNewSprite(strL));
                count++;
            }

            foreach (List<string> strL in sprite_definitions_old)
            {
                sprite_definitions_old_renamed.AddRange(AdjustSpriteName(strL));
            }

            //Build a list of sprites we need to alias
            foreach (string spritestr in existing_sprite_codes)
            {
                char eyes = spritestr[1];
                if (eyes == 'h' || eyes == 'd' || eyes == 'k' || eyes == 'n')
                    codes_to_alias.Add(spritestr);
            }

            sprite_chart_new.AddRange(sprite_definitions_old_renamed);

            //Now we get everything up to the aliases
            List<string> curr_aliases = new List<string>();
            line_count = 0;
            foreach (string s in sprite_chart_old)
            {
                if (s == "### [IMG040]")
                    break;

                curr_aliases.Add(s);
                line_count++;
            }
            sprite_chart_old.RemoveRange(0,line_count);

            sprite_chart_new.AddRange(curr_aliases);
            sprite_chart_new.Add("#closedhappy/closedsad/wink aliases");

            codes_to_alias.Sort();
            char last_pose_numb = '1';
            sprite_chart_new.Add("#Pose 1");
            foreach (string s in codes_to_alias)
            {
                if (s[0] != last_pose_numb)
                {
                    sprite_chart_new.Add("");
                    sprite_chart_new.Add($"#Pose {s[0]}");
                    last_pose_numb = s[0];
                }
                sprite_chart_new.Add(BuildSpriteAlias(s));
            }

            sprite_chart_new.Add("");
            sprite_chart_new.AddRange(sprite_chart_old);
            sprite_chart_two.AddRange(sprite_definitions_new);

            file.create("sprite-chart.rpy","New",sprite_chart_new,overwrite: true);
            file.create("sprite-chart-01.rpy","New",sprite_chart_two,overwrite: true);
        }

        public static string BuildSpriteAlias(string spritecode)
        {
            return $"image monika {spritecode} = \"monika {spritecode + "_static"}\"";
        }

        public static List<string> AdjustSpriteName(List<string> old_sprite_def)
        {
            old_sprite_def[0] = old_sprite_def[0].Replace(GetSpriteCode(old_sprite_def[0]),GetSpriteCode(old_sprite_def[0]) + "_static");
            return old_sprite_def;
        }

        public static List<string> CreateNewSprite(List<string> old_sprite_def)
        {
            //So we don't do extra work
            foreach (string s in old_sprite_def)
            {
                if (s.Contains("closedsad") || s.Contains("closedhappy") || s.Contains("wink"))
                    return old_sprite_def;
            }

            List<string> new_sprite_def = new List<string>();
            MonikaSprite spr = GetSpriteFromCode(GetSpriteCode(old_sprite_def[0]));
            spr.eyes = "closedsad";

            new_sprite_def.Add($"image monika {GetSpriteCode(old_sprite_def[0])}:");
            new_sprite_def.Add("    block:");
            new_sprite_def.Add($"        \"monika {GetSpriteCode(old_sprite_def[0])}_static\"");
            new_sprite_def.Add("        block:");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                5");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                7");
            new_sprite_def.Add("            choice:");
            new_sprite_def.Add("                9");
            new_sprite_def.Add($"        \"monika {GetSpriteCodeFromObject(spr)}_static\"");
            new_sprite_def.Add("        0.05");
            new_sprite_def.Add("        repeat");
            new_sprite_def.Add("");
            return new_sprite_def;
        }

        public static string GetSpriteCode(string line)
        {
            return line.Substring(13, line.IndexOf("=") - 14);
        }

        public static MonikaSprite GetSpriteFromCode(string spritecode)
        {
            string eyes="", eyebrows="", mouth="", sweat="", blush="", tears="", arms="", left="", right="", head="";

            //Sweat drop
            if (spritecode.Contains("sdl"))
            {
                sweat = "def";
                spritecode.Replace("sdl","");
            }
            else if (spritecode.Contains("sdr"))
            {
                sweat = "right";
                spritecode.Replace("sdr","");
            }

            //Blush
            if (spritecode.Contains("bf"))
            {
                blush = "full";
                spritecode.Replace("bf","");
            }
            else if (spritecode.Contains("bs"))
            {
                blush = "shade";
                spritecode.Replace("bs","");
            }
            else if (spritecode.Contains("bl"))
            {
                blush = "lines";
                spritecode.Replace("bl","");
            }

            //Tears
            if (spritecode.Contains("ts"))
            {
                tears = "streaming";
                spritecode.Replace("ts","");
            }
            else if (spritecode.Contains("td"))
            {
                tears = "dried";
                spritecode.Replace("td","");
            }
            else if (spritecode.Contains("tl"))
            {
                tears = "left";
                spritecode.Replace("tl","");
            }
            else if (spritecode.Contains("tr"))
            {
                tears = "right";
                spritecode.Replace("tr","");
            }
            else if (spritecode.Contains("tp"))
            {
                tears = "pooled";
                spritecode.Replace("tp","");
            }
            else if (spritecode.Contains("tu"))
            {
                tears = "up";
                spritecode.Replace("tu","");
            }

            //Eyes
            switch (spritecode[1])
            {
                case 'e':
                    eyes = "normal";
                    break;
                case 'w':
                    eyes = "wide";
                    break;
                case 's':
                    eyes = "sparkle";
                    break;
                case 't':
                    eyes = "smug";
                    break;
                case 'c':
                    eyes = "crazy";
                    break;
                case 'r':
                    eyes = "right";
                    break;
                case 'l':
                    eyes = "left";
                    break;
                case 'h':
                    eyes = "closedhappy";
                    break;
                case 'd':
                    eyes = "closedsad";
                    break;
                case 'k':
                    eyes = "winkleft";
                    break;
                case 'n':
                    eyes = "winkright";
                    break;
            }

            //Eyebrows
            switch (spritecode[2])
            {
                case 'f':
                    eyebrows = "furrowed";
                    break;
                case 'u':
                    eyebrows = "up";
                    break;
                case 'k':
                    eyebrows = "knit";
                    break;
                case 's':
                    eyebrows = "mid";
                    break;
                case 't':
                    eyebrows = "think";
                    break;
            }

            //Mouth
            switch (spritecode[spritecode.Length-1])
            {
                case 'a':
                    mouth = "smile";
                    head = "a";
                    break;
                case 'b':
                    mouth = "big";
                    head = "b";
                    break;
                case 'c':
                    mouth = "smirk";
                    head = "h";
                    break;
                case 'd':
                    mouth = "small";
                    head = "i";
                    break;
                case 'o':
                    mouth = "gasp";
                    head = "d";
                    break;
                case 'u':
                    mouth = "smug";
                    head = "j";
                    break;
                case 'w':
                    mouth = "wide";
                    head = "b";
                    break;
                case 'x':
                    mouth = "disgust";
                    head = "f";
                    break;
                case 'p':
                    mouth = "pout";
                    head = "h";
                    break;
                case 't':
                    mouth = "triangle";
                    head = "a";
                    break;
            }

            //Arms
            switch (spritecode[0])
            {
                case '1':
                    arms = "steepling";
                    left = "1l";
                    right = "1r";
                    break;
                case '2':
                    arms = "crossed";
                    left = "1l";
                    right = "2r";
                    break;
                case '3':
                    arms = "restleftpointright";
                    left = "2l";
                    right = "1r";
                    break;
                case '4':
                    arms = "pointright";
                    left = "2l";
                    right = "2r";
                    break;
                case '5':
                    arms = "def";
                    head = "";
                    break;
                case '6':
                    arms = "down";
                    left = "1l";
                    right = "1r";
                    break;
            }

            return new MonikaSprite(
                eyebrows: eyebrows,
                eyes: eyes,
                mouth: mouth,
                arms: arms,
                blush: blush,
                tears: tears,
                sweat: sweat,
                head: head,
                left: left,
                right: right
            );
        }

        public static string GetSpriteCodeFromObject(MonikaSprite spr)
        {
            string arms="", eyes="", eyebrows="", tears="", sweat="", blush="", mouth="";

            //Arms
            switch (spr.arms)
            {
                case "steepling":
                    arms="1";
                    break;
                case "crossed":
                    arms="2";
                    break;
                case "restleftpointright":
                    arms="3";
                    break;
                case "pointright":
                    arms="4";
                    break;
                case "def":
                    arms="5";
                    break;
                case "down":
                    arms="6";
                    break;
            }

            //Eyes
            switch (spr.eyes)
            {
                case "normal":
                    eyes="e";
                    break;
                case "wide":
                    eyes="w";
                    break;
                case "sparkle":
                    eyes="s";
                    break;
                case "smug":
                    eyes="t";
                    break;
                case "crazy":
                    eyes="c";
                    break;
                case "right":
                    eyes="r";
                    break;
                case "left":
                    eyes="l";
                    break;
                case "closedhappy":
                    eyes="h";
                    break;
                case "closedsad":
                    eyes="d";
                    break;
                case "winkleft":
                    eyes="k";
                    break;
                case "winkright":
                    eyes="n";
                    break;
            }

            //Eyebrows
            switch (spr.eyebrows)
            {
                case "furrowed":
                    eyebrows="f";
                    break;
                case "up":
                    eyebrows="u";
                    break;
                case "knit":
                    eyebrows="k";
                    break;
                case "mid":
                    eyebrows="s";
                    break;
                case "thinking":
                    eyebrows="t";
                    break;
            }

            //Blush
            switch (spr.blush)
            {
                case "full":
                    blush="bf";
                    break;
                case "shade":
                    blush="bs";
                    break;
                case "lines":
                    blush="bl";
                    break;
                default:
                    blush="";
                    break;
            }

            //Tears
            switch (spr.tears)
            {
                case "streaming":
                    tears="ts";
                    break;
                case "dried":
                    tears="td";
                    break;
                case "left":
                    tears="tl";
                    break;
                case "right":
                    tears="tr";
                    break;
                case "pooled":
                    tears="tp";
                    break;
                case "up":
                    tears="tu";
                    break;
                default:
                    tears="";
                    break;
            }

            //Sweat drop
            switch (spr.sweat)
            {
                case "def":
                    sweat="sdl";
                    break;
                case "right":
                    sweat="sdr";
                    break;
                default:
                    sweat="";
                    break;
            }

            //Mouth
            switch (spr.mouth)
            {
                case "smile":
                    mouth="a";
                    break;
                case "big":
                    mouth="b";
                    break;
                case "smirk":
                    mouth="c";
                    break;
                case "small":
                    mouth="d";
                    break;
                case "gasp":
                    mouth="o";
                    break;
                case "smug":
                    mouth="u";
                    break;
                case "wide":
                    mouth="w";
                    break;
                case "disgust":
                    mouth="x";
                    break;
                case "pout":
                    mouth="p";
                    break;
                case "triangle":
                    mouth="t";
                    break;
            }

            return arms + eyes + eyebrows + blush + tears + sweat + mouth;
        }

        public static List<string> BuildOldSprite(MonikaSprite spr)
        {
            List<string> sprite_def = new List<string>();

            sprite_def.Add($"image monika {GetSpriteCodeFromObject(spr)}_static = DynamicDisplayable(");
            sprite_def.Add($"    {spr.method},");
            sprite_def.Add($"    {spr.character},");
            sprite_def.Add($"    eyes=\"{spr.eyes}\",");
            sprite_def.Add($"    eyebrows=\"{spr.eyes}\",");
            sprite_def.Add($"    nose=\"{spr.nose}\",");
            sprite_def.Add($"    mouth=\"{spr.mouth}\",");

            if (spr.blush != "")
                sprite_def.Add($"    blush=\"{spr.blush}\",");
            if (spr.tears != "")
                sprite_def.Add($"    tears=\"{spr.tears}\",");
            if (spr.sweat != "")
                sprite_def.Add($"    sweat=\"{spr.sweat}\",");

            sprite_def.Add($"    head=\"{spr.head}\",");
            sprite_def.Add($"    left=\"{spr.left}\",");
            sprite_def.Add($"    right=\"{spr.right}\",");
            sprite_def.Add($"    arms=\"{spr.arms}\",");
            sprite_def.Add(")");
            sprite_def.Add("");
            return sprite_def;
        }
    }

    public class MonikaSprite
    {
        public string method = "mas_drawmonika";
        public string character = "monika_chr";
        public string eyebrows;
        public string eyes;
        public string nose = "def";
        public string mouth;
        public string head = "";
        public string left = "";
        public string right = "";
        public string arms;
        public string tears = "";
        public string blush = "";
        public string sweat = "";
        public string eyebags = "";

        public MonikaSprite(string eyebrows, string eyes, string mouth, string arms, string head="", string left="", string right="", string method="mas_drawmonika",string character="monika_chr", string nose="def", string tears="", string sweat="", string blush="")
        {
            this.method = "mas_drawmonika";
            this.character = "monika_chr";
            this.eyes = eyes;
            this.eyebrows = eyebrows;
            this.nose = nose;
            this.blush = blush;
            this.sweat = sweat;
            this.tears = tears;
            this.mouth = mouth;
            this.arms = arms;
        }
    }
}
