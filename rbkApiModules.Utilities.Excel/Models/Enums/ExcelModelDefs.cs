using System;
namespace rbkApiModules.Utilities.Excel;

public static class ExcelModelDefs
{
    public static class StyleContants
    {
        public static UInt32 StartIndex
        {
            get
            {
                return 164;
            }
        }
    }

    public static class ExcelSheetTypes
    {
        public enum Type
        {
            Table = 0,
            Chart = 1
        }
    }

    public static class ExcelThemes
    {
        public enum Theme
        {
            None = 0,
            TableStyleLight1 = 1,
            TableStyleLight2 = 2,
            TableStyleLight3 = 3,
            TableStyleLight4 = 4,
            TableStyleLight5 = 5,
            TableStyleLight6 = 6,
            TableStyleLight7 = 7,
            TableStyleLight8 = 8,
            TableStyleLight9 = 9,
            TableStyleLight10 = 10,
            TableStyleLight11 = 11,
            TableStyleLight12 = 12,
            TableStyleLight13 = 13,
            TableStyleLight14 = 14,
            TableStyleLight15 = 15,
            TableStyleLight16 = 16,
            TableStyleLight17 = 17,
            TableStyleLight18 = 18,
            TableStyleLight19 = 19,
            TableStyleLight20 = 20,
            TableStyleLight21 = 21,
            TableStyleMedium1 = 22,
            TableStyleMedium2 = 23,
            TableStyleMedium3 = 24,
            TableStyleMedium4 = 25,
            TableStyleMedium5 = 26,
            TableStyleMedium6 = 27,
            TableStyleMedium7 = 28,
            TableStyleMedium8 = 29,
            TableStyleMedium9 = 30,
            TableStyleMedium10 = 31,
            TableStyleMedium11 = 32,
            TableStyleMedium12 = 33,
            TableStyleMedium13 = 34,
            TableStyleMedium14 = 35,
            TableStyleMedium15 = 36,
            TableStyleMedium16 = 37,
            TableStyleMedium17 = 38,
            TableStyleMedium18 = 39,
            TableStyleMedium19 = 40,
            TableStyleMedium20 = 41,
            TableStyleMedium21 = 42,
            TableStyleDark1 = 43,
            TableStyleDark2 = 44,
            TableStyleDark3 = 45,
            TableStyleDark4 = 46,
            TableStyleDark5 = 47,
            TableStyleDark6 = 48,
            TableStyleDark7 = 49,
            TableStyleDark8 = 50,
            TableStyleDark9 = 51,
            TableStyleDark10 = 52,
            TableStyleDark11 = 53
        }

        public static string GetTheme(Theme theme)
        {
            switch ((int)theme)
            {
                case 0: return "None";
                case 1: return "TableStyleLight1";
                case 2: return "TableStyleLight2";
                case 3: return "TableStyleLight3";
                case 4: return "TableStyleLight4";
                case 5: return "TableStyleLight5";
                case 6: return "TableStyleLight6";
                case 7: return "TableStyleLight7";
                case 8: return "TableStyleLight8";
                case 9: return "TableStyleLight9";
                case 10: return "TableStyleLight10";
                case 11: return "TableStyleLight11";
                case 12: return "TableStyleLight12";
                case 13: return "TableStyleLight13";
                case 14: return "TableStyleLight14";
                case 15: return "TableStyleLight15";
                case 16: return "TableStyleLight16";
                case 17: return "TableStyleLight17";
                case 18: return "TableStyleLight18";
                case 19: return "TableStyleLight19";
                case 20: return "TableStyleLight20";
                case 21: return "TableStyleLight21";
                case 22: return "TableStyleMedium1";
                case 23: return "TableStyleMedium2";
                case 24: return "TableStyleMedium3";
                case 25: return "TableStyleMedium4";
                case 26: return "TableStyleMedium5";
                case 27: return "TableStyleMedium6";
                case 28: return "TableStyleMedium7";
                case 29: return "TableStyleMedium8";
                case 30: return "TableStyleMedium9";
                case 31: return "TableStyleMedium10";
                case 32: return "TableStyleMedium11";
                case 33: return "TableStyleMedium12";
                case 34: return "TableStyleMedium13";
                case 35: return "TableStyleMedium14";
                case 36: return "TableStyleMedium15";
                case 37: return "TableStyleMedium16";
                case 38: return "TableStyleMedium17";
                case 39: return "TableStyleMedium18";
                case 40: return "TableStyleMedium19";
                case 41: return "TableStyleMedium20";
                case 42: return "TableStyleMedium21";
                case 43: return "TableStyleDark1";
                case 44: return "TableStyleDark2";
                case 45: return "TableStyleDark3";
                case 46: return "TableStyleDark4";
                case 47: return "TableStyleDark5";
                case 48: return "TableStyleDark6";
                case 49: return "TableStyleDark7";
                case 50: return "TableStyleDark8";
                case 51: return "TableStyleDark9";
                case 52: return "TableStyleDark10";
                case 53: return "TableStyleDark11";
                default:
                    return "None";
            }
        }
    }

    public static class ExcelFonts
    {
        public enum FontType
        {
            Arial = 0,
            Calibri = 1,
            CalibriLight = 2,
            CourierNew = 3,
            TimesNewRoman = 4
        }

        public static double GetFontSizeFactor(FontType fontType)
        {
            switch ((int)fontType)
            {
                case 0: return 8D;
                case 1: return 8.5D;
                case 2: return 8.5D;
                case 3: return 8.5D;
                case 4: return 8.5D;
                default: return 8.5D;
            }
        }
    }

    public static class ExcelDataTypes
    {
        public enum DataType
        {
            Text = 0,
            Number = 1,
            DateTime = 2,
            HyperLink = 3,
            AutoDetect = 4
        }
    }

    public static class ExcelSort
    {
        public enum SortOrder
        {
            Ascending = 0,
            Descending = 1
        }
    }
    public static class ExcelColors
    {
        public enum Color
        {
            NoColor = 0,
            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen,
            AirForceBlue,
            Alizarin,
            Almond,
            Amaranth,
            Amber,
            AmberSaeEce,
            AmericanRose,
            Amethyst,
            AntiFlashWhite,
            AntiqueBrass,
            AntiqueFuchsia,
            AppleGreen,
            Apricot,
            Aquamarine1,
            ArmyGreen,
            Arsenic,
            ArylideYellow,
            AshGrey,
            Asparagus,
            AtomicTangerine,
            Auburn,
            Aureolin,
            Aurometalsaurus,
            Awesome,
            AzureColorWheel,
            BabyBlue,
            BabyBlueEyes,
            BabyPink,
            BallBlue,
            BananaMania,
            BattleshipGrey,
            Bazaar,
            BeauBlue,
            Beaver,
            Bistre,
            Bittersweet,
            BleuDeFrance,
            BlizzardBlue,
            Blond,
            BlueBell,
            BlueGray,
            BlueGreen,
            BluePigment,
            BlueRyb,
            Blush,
            Bole,
            BondiBlue,
            BostonUniversityRed,
            BrandeisBlue,
            Brass,
            BrickRed,
            BrightCerulean,
            BrightGreen,
            BrightLavender,
            BrightMaroon,
            BrightPink,
            BrightTurquoise,
            BrightUbe,
            BrilliantLavender,
            BrilliantRose,
            BrinkPink,
            BritishRacingGreen,
            Bronze,
            BrownTraditional,
            BubbleGum,
            Bubbles,
            Buff,
            BulgarianRose,
            Burgundy,
            BurntOrange,
            BurntSienna,
            BurntUmber,
            Byzantine,
            Byzantium,
            Cadet,
            CadetGrey,
            CadmiumGreen,
            CadmiumOrange,
            CadmiumRed,
            CadmiumYellow,
            CalPolyPomonaGreen,
            CambridgeBlue,
            Camel,
            CamouflageGreen,
            CanaryYellow,
            CandyAppleRed,
            CandyPink,
            CaputMortuum,
            Cardinal,
            CaribbeanGreen,
            Carmine,
            CarminePink,
            CarmineRed,
            CarnationPink,
            Carnelian,
            CarolinaBlue,
            CarrotOrange,
            Ceil,
            Celadon,
            CelestialBlue,
            Cerise,
            CerisePink,
            Cerulean,
            CeruleanBlue,
            Chamoisee,
            Champagne,
            Charcoal,
            ChartreuseTraditional,
            CherryBlossomPink,
            Chocolate1,
            ChromeYellow,
            Cinereous,
            Cinnabar,
            Citrine,
            ClassicRose,
            Cobalt,
            ColumbiaBlue,
            CoolBlack,
            CoolGrey,
            Copper,
            CopperRose,
            Coquelicot,
            CoralPink,
            CoralRed,
            Cordovan,
            Corn,
            CornellRed,
            CosmicLatte,
            CottonCandy,
            Cream,
            CrimsonGlory,
            CyanProcess,
            Daffodil,
            Dandelion,
            DarkBrown,
            DarkByzantium,
            DarkCandyAppleRed,
            DarkCerulean,
            DarkChampagne,
            DarkChestnut,
            DarkCoral,
            DarkElectricBlue,
            DarkGreen1,
            DarkJungleGreen,
            DarkLava,
            DarkLavender,
            DarkMidnightBlue,
            DarkPastelBlue,
            DarkPastelGreen,
            DarkPastelPurple,
            DarkPastelRed,
            DarkPink,
            DarkPowderBlue,
            DarkRaspberry,
            DarkScarlet,
            DarkSienna,
            DarkSpringGreen,
            DarkTan,
            DarkTangerine,
            DarkTaupe,
            DarkTerraCotta,
            DartmouthGreen,
            DavysGrey,
            DebianRed,
            DeepCarmine,
            DeepCarminePink,
            DeepCarrotOrange,
            DeepCerise,
            DeepChampagne,
            DeepChestnut,
            DeepFuchsia,
            DeepJungleGreen,
            DeepLilac,
            DeepMagenta,
            DeepPeach,
            DeepSaffron,
            Denim,
            Desert,
            DesertSand,
            DogwoodRose,
            DollarBill,
            Drab,
            DukeBlue,
            EarthYellow,
            Ecru,
            Eggplant,
            Eggshell,
            EgyptianBlue,
            ElectricBlue,
            ElectricCrimson,
            ElectricIndigo,
            ElectricLavender,
            ElectricLime,
            ElectricPurple,
            ElectricUltramarine,
            ElectricViolet,
            Emerald,
            EtonBlue,
            Fallow,
            FaluRed,
            Fandango,
            FashionFuchsia,
            Fawn,
            Feldgrau,
            FernGreen,
            FerrariRed,
            FieldDrab,
            FireEngineRed,
            Flame,
            FlamingoPink,
            Flavescent,
            Flax,
            FluorescentOrange,
            FluorescentYellow,
            Folly,
            ForestGreenTraditional,
            FrenchBeige,
            FrenchBlue,
            FrenchLilac,
            FrenchRose,
            FuchsiaPink,
            Fulvous,
            FuzzyWuzzy,
            Gamboge,
            Ginger,
            Glaucous,
            GoldenBrown,
            GoldenPoppy,
            GoldenYellow,
            GoldMetallic,
            GrannySmithApple,
            GrayAsparagus,
            GreenPigment,
            GreenRyb,
            Grullo,
            HalayaUbe,
            HanBlue,
            HanPurple,
            HansaYellow,
            Harlequin,
            HarvardCrimson,
            HarvestGold,
            Heliotrope,
            HollywoodCerise,
            HookersGreen,
            HotMagenta,
            HunterGreen,
            Iceberg,
            Icterine,
            Inchworm,
            IndiaGreen,
            IndianYellow,
            IndigoDye,
            InternationalKleinBlue,
            InternationalOrange,
            Iris,
            Isabelline,
            IslamicGreen,
            Jade,
            Jasper,
            JazzberryJam,
            Jonquil,
            JuneBud,
            JungleGreen,
            KellyGreen,
            KhakiHtmlCssKhaki,
            LanguidLavender,
            LapisLazuli,
            LaSalleGreen,
            LaserLemon,
            Lava,
            LavenderBlue,
            LavenderFloral,
            LavenderGray,
            LavenderIndigo,
            LavenderPink,
            LavenderPurple,
            LavenderRose,
            Lemon,
            LightApricot,
            LightBrown,
            LightCarminePink,
            LightCornflowerBlue,
            LightFuchsiaPink,
            LightMauve,
            LightPastelPurple,
            LightSalmonPink,
            LightTaupe,
            LightThulianPink,
            LightYellow1,
            Lilac,
            LimeColorWheel,
            LincolnGreen,
            Liver,
            Lust,
            MacaroniAndCheese,
            MagentaDye,
            MagentaProcess,
            MagicMint,
            Magnolia,
            Mahogany,
            Maize,
            MajorelleBlue,
            Malachite,
            Manatee,
            MangoTango,
            MaroonX11,
            Mauve,
            Mauvelous,
            MauveTaupe,
            MayaBlue,
            MeatBrown,
            MediumAquamarine1,
            MediumCandyAppleRed,
            MediumCarmine,
            MediumChampagne,
            MediumElectricBlue,
            MediumJungleGreen,
            MediumPersianBlue,
            MediumRedViolet,
            MediumSpringBud,
            MediumTaupe,
            Melon,
            MidnightGreenEagleGreen,
            MikadoYellow,
            Mint,
            MintGreen,
            ModeBeige,
            MoonstoneBlue,
            MordantRed19,
            MossGreen,
            MountainMeadow,
            MountbattenPink,
            MsuGreen,
            Mulberry,
            Mustard,
            Myrtle,
            NadeshikoPink,
            NapierGreen,
            NaplesYellow,
            NeonCarrot,
            NeonFuchsia,
            NeonGreen,
            NonPhotoBlue,
            OceanBoatBlue,
            Ochre,
            OldGold,
            OldLavender,
            OldMauve,
            OldRose,
            OliveDrab7,
            Olivine,
            Onyx,
            OperaMauve,
            OrangeColorWheel,
            OrangePeel,
            OrangeRyb,
            OtterBrown,
            OuCrimsonRed,
            OuterSpace,
            OutrageousOrange,
            OxfordBlue,
            PakistanGreen,
            PalatinateBlue,
            PalatinatePurple,
            PaleAqua,
            PaleBrown,
            PaleCarmine,
            PaleCerulean,
            PaleChestnut,
            PaleCopper,
            PaleCornflowerBlue,
            PaleGold,
            PaleMagenta,
            PalePink,
            PaleRobinEggBlue,
            PaleSilver,
            PaleSpringBud,
            PaleTaupe,
            PansyPurple,
            ParisGreen,
            PastelBlue,
            PastelBrown,
            PastelGray,
            PastelGreen,
            PastelMagenta,
            PastelOrange,
            PastelPink,
            PastelPurple,
            PastelRed,
            PastelViolet,
            PastelYellow,
            PaynesGrey,
            Peach,
            PeachOrange,
            PeachYellow,
            Pear,
            Pearl,
            Peridot,
            Periwinkle,
            PersianBlue,
            PersianGreen,
            PersianIndigo,
            PersianOrange,
            PersianPink,
            PersianPlum,
            PersianRed,
            PersianRose,
            Persimmon,
            Phlox,
            PhthaloBlue,
            PhthaloGreen,
            PiggyPink,
            PineGreen,
            PinkOrange,
            PinkPearl,
            PinkSherbet,
            Pistachio,
            Platinum,
            PlumTraditional,
            PortlandOrange,
            PrincetonOrange,
            Prune,
            PrussianBlue,
            PsychedelicPurple,
            Puce,
            Pumpkin,
            PurpleHeart,
            PurpleMountainMajesty,
            PurpleMunsell,
            PurplePizzazz,
            PurpleTaupe,
            PurpleX11,
            RadicalRed,
            Raspberry,
            RaspberryGlace,
            RaspberryPink,
            RaspberryRose,
            RawUmber,
            RazzleDazzleRose,
            Razzmatazz,
            RedMunsell,
            RedNcs,
            RedPigment,
            RedRyb,
            Redwood,
            Regalia,
            RichBlack,
            RichBrilliantLavender,
            RichCarmine,
            RichElectricBlue,
            RichLavender,
            RichLilac,
            RichMaroon,
            RifleGreen,
            RobinEggBlue,
            Rose,
            RoseBonbon,
            RoseEbony,
            RoseGold,
            RoseMadder,
            RosePink,
            RoseQuartz,
            RoseTaupe,
            RoseVale,
            Rosewood,
            RossoCorsa,
            RoyalAzure,
            RoyalBlueTraditional,
            RoyalFuchsia,
            RoyalPurple,
            Ruby,
            Ruddy,
            RuddyBrown,
            RuddyPink,
            Rufous,
            Russet,
            Rust,
            SacramentoStateGreen,
            SafetyOrangeBlazeOrange,
            Saffron,
            Salmon1,
            SalmonPink,
            Sand,
            SandDune,
            Sandstorm,
            SandyTaupe,
            Sangria,
            SapGreen,
            Sapphire,
            SatinSheenGold,
            Scarlet,
            SchoolBusYellow,
            ScreaminGreen,
            SealBrown,
            SelectiveYellow,
            Sepia,
            Shadow,
            ShamrockGreen,
            ShockingPink,
            Sienna1,
            Sinopia,
            Skobeloff,
            SkyMagenta,
            SmaltDarkPowderBlue,
            SmokeyTopaz,
            SmokyBlack,
            SpiroDiscoBall,
            SplashedWhite,
            SpringBud,
            StPatricksBlue,
            StilDeGrainYellow,
            Straw,
            Sunglow,
            Sunset,
            Tangelo,
            Tangerine,
            TangerineYellow,
            Taupe,
            TaupeGray,
            TeaGreen,
            TealBlue,
            TealGreen,
            TeaRoseOrange,
            TeaRoseRose,
            TennéTawny,
            TerraCotta,
            ThulianPink,
            TickleMePink,
            TiffanyBlue,
            TigersEye,
            Timberwolf,
            TitaniumYellow,
            Toolbox,
            TractorRed,
            TropicalRainForest,
            TuftsBlue,
            Tumbleweed,
            TurkishRose,
            Turquoise1,
            TurquoiseBlue,
            TurquoiseGreen,
            TuscanRed,
            TwilightLavender,
            TyrianPurple,
            UaBlue,
            UaRed,
            Ube,
            UclaBlue,
            UclaGold,
            UfoGreen,
            Ultramarine,
            UltramarineBlue,
            UltraPink,
            Umber,
            UnitedNationsBlue,
            UnmellowYellow,
            UpForestGreen,
            UpMaroon,
            UpsdellRed,
            Urobilin,
            UscCardinal,
            UscGold,
            UtahCrimson,
            Vanilla,
            VegasGold,
            VenetianRed,
            Verdigris,
            Vermilion,
            Veronica,
            Violet1,
            VioletColorWheel,
            VioletRyb,
            Viridian,
            VividAuburn,
            VividBurgundy,
            VividCerise,
            VividTangerine,
            VividViolet,
            WarmBlack,
            Wenge,
            WildBlueYonder,
            WildStrawberry,
            WildWatermelon,
            Wisteria,
            Xanadu,
            YaleBlue,
            YellowMunsell,
            YellowNcs,
            YellowProcess,
            YellowRyb,
            Zaffre,
            ZinnwalditeBrown,
            Transparent,
        }

        //public static XLColor GetColor(Color color)
        //{
        //    switch((int) color)
        //    {
        //        case 0: return XLColor.NoColor;
        //        case 1: return XLColor.AliceBlue;
        //        case 2: return XLColor.AntiqueWhite;
        //        case 3: return XLColor.Aqua;
        //        case 4: return XLColor.Aquamarine;
        //        case 5: return XLColor.Azure;
        //        case 6: return XLColor.Beige;
        //        case 7: return XLColor.Bisque;
        //        case 8: return XLColor.Black;
        //        case 9: return XLColor.BlanchedAlmond;
        //        case 10: return XLColor.Blue;
        //        case 11: return XLColor.BlueViolet;
        //        case 12: return XLColor.Brown;
        //        case 13: return XLColor.BurlyWood;
        //        case 14: return XLColor.CadetBlue;
        //        case 15: return XLColor.Chartreuse;
        //        case 16: return XLColor.Chocolate;
        //        case 17: return XLColor.Coral;
        //        case 18: return XLColor.CornflowerBlue;
        //        case 19: return XLColor.Cornsilk;
        //        case 20: return XLColor.Crimson;
        //        case 21: return XLColor.Cyan;
        //        case 22: return XLColor.DarkBlue;
        //        case 23: return XLColor.DarkCyan;
        //        case 24: return XLColor.DarkGoldenrod;
        //        case 25: return XLColor.DarkGray;
        //        case 26: return XLColor.DarkGreen;
        //        case 27: return XLColor.DarkKhaki;
        //        case 28: return XLColor.DarkMagenta;
        //        case 29: return XLColor.DarkOliveGreen;
        //        case 30: return XLColor.DarkOrange;
        //        case 31: return XLColor.DarkOrchid;
        //        case 32: return XLColor.DarkRed;
        //        case 33: return XLColor.DarkSalmon;
        //        case 34: return XLColor.DarkSeaGreen;
        //        case 35: return XLColor.DarkSlateBlue;
        //        case 36: return XLColor.DarkSlateGray;
        //        case 37: return XLColor.DarkTurquoise;
        //        case 38: return XLColor.DarkViolet;
        //        case 39: return XLColor.DeepPink;
        //        case 40: return XLColor.DeepSkyBlue;
        //        case 41: return XLColor.DimGray;
        //        case 42: return XLColor.DodgerBlue;
        //        case 43: return XLColor.Firebrick;
        //        case 44: return XLColor.FloralWhite;
        //        case 45: return XLColor.ForestGreen;
        //        case 46: return XLColor.Fuchsia;
        //        case 47: return XLColor.Gainsboro;
        //        case 48: return XLColor.GhostWhite;
        //        case 49: return XLColor.Gold;
        //        case 50: return XLColor.Goldenrod;
        //        case 51: return XLColor.Gray;
        //        case 52: return XLColor.Green;
        //        case 53: return XLColor.GreenYellow;
        //        case 54: return XLColor.Honeydew;
        //        case 55: return XLColor.HotPink;
        //        case 56: return XLColor.IndianRed;
        //        case 57: return XLColor.Indigo;
        //        case 58: return XLColor.Ivory;
        //        case 59: return XLColor.Khaki;
        //        case 60: return XLColor.Lavender;
        //        case 61: return XLColor.LavenderBlush;
        //        case 62: return XLColor.LawnGreen;
        //        case 63: return XLColor.LemonChiffon;
        //        case 64: return XLColor.LightBlue;
        //        case 65: return XLColor.LightCoral;
        //        case 66: return XLColor.LightCyan;
        //        case 67: return XLColor.LightGoldenrodYellow;
        //        case 68: return XLColor.LightGray;
        //        case 69: return XLColor.LightGreen;
        //        case 70: return XLColor.LightPink;
        //        case 71: return XLColor.LightSalmon;
        //        case 72: return XLColor.LightSeaGreen;
        //        case 73: return XLColor.LightSkyBlue;
        //        case 74: return XLColor.LightSlateGray;
        //        case 75: return XLColor.LightSteelBlue;
        //        case 76: return XLColor.LightYellow;
        //        case 77: return XLColor.Lime;
        //        case 78: return XLColor.LimeGreen;
        //        case 79: return XLColor.Linen;
        //        case 80: return XLColor.Magenta;
        //        case 91: return XLColor.Maroon;
        //        case 92: return XLColor.MediumAquamarine;
        //        case 93: return XLColor.MediumBlue;
        //        case 94: return XLColor.MediumOrchid;
        //        case 95: return XLColor.MediumPurple;
        //        case 96: return XLColor.MediumSeaGreen;
        //        case 97: return XLColor.MediumSlateBlue;
        //        case 98: return XLColor.MediumSpringGreen;
        //        case 99: return XLColor.MediumTurquoise;
        //        case 100: return XLColor.MediumVioletRed;
        //        case 101: return XLColor.MidnightBlue;
        //        case 102: return XLColor.MintCream;
        //        case 103: return XLColor.MistyRose;
        //        case 104: return XLColor.Moccasin;
        //        case 105: return XLColor.NavajoWhite;
        //        case 106: return XLColor.Navy;
        //        case 107: return XLColor.OldLace;
        //        case 108: return XLColor.Olive;
        //        case 109: return XLColor.OliveDrab;
        //        case 110: return XLColor.Orange;
        //        case 111: return XLColor.OrangeRed;
        //        case 112: return XLColor.Orchid;
        //        case 113: return XLColor.PaleGoldenrod;
        //        case 114: return XLColor.PaleGreen;
        //        case 115: return XLColor.PaleTurquoise;
        //        case 116: return XLColor.PaleVioletRed;
        //        case 117: return XLColor.PapayaWhip;
        //        case 118: return XLColor.PeachPuff;
        //        case 119: return XLColor.Peru;
        //        case 120: return XLColor.Pink;
        //        case 121: return XLColor.Plum;
        //        case 122: return XLColor.PowderBlue;
        //        case 123: return XLColor.Purple;
        //        case 124: return XLColor.Red;
        //        case 125: return XLColor.RosyBrown;
        //        case 126: return XLColor.RoyalBlue;
        //        case 127: return XLColor.SaddleBrown;
        //        case 128: return XLColor.Salmon;
        //        case 129: return XLColor.SandyBrown;
        //        case 130: return XLColor.SeaGreen;
        //        case 131: return XLColor.SeaShell;
        //        case 132: return XLColor.Sienna;
        //        case 133: return XLColor.Silver;
        //        case 134: return XLColor.SkyBlue;
        //        case 135: return XLColor.SlateBlue;
        //        case 136: return XLColor.SlateGray;
        //        case 137: return XLColor.Snow;
        //        case 138: return XLColor.SpringGreen;
        //        case 139: return XLColor.SteelBlue;
        //        case 140: return XLColor.Tan;
        //        case 141: return XLColor.Teal;
        //        case 142: return XLColor.Thistle;
        //        case 143: return XLColor.Tomato;
        //        case 144: return XLColor.Turquoise;
        //        case 145: return XLColor.Violet;
        //        case 146: return XLColor.Wheat;
        //        case 147: return XLColor.White;
        //        case 148: return XLColor.WhiteSmoke;
        //        case 149: return XLColor.Yellow;
        //        case 150: return XLColor.YellowGreen;
        //        case 151: return XLColor.AirForceBlue;
        //        case 152: return XLColor.Alizarin;
        //        case 153: return XLColor.Almond;
        //        case 154: return XLColor.Amaranth;
        //        case 155: return XLColor.Amber;
        //        case 156: return XLColor.AmberSaeEce;
        //        case 157: return XLColor.AmericanRose;
        //        case 158: return XLColor.Amethyst;
        //        case 159: return XLColor.AntiFlashWhite;
        //        case 160: return XLColor.AntiqueBrass;
        //        case 161: return XLColor.AntiqueFuchsia;
        //        case 168: return XLColor.AppleGreen;
        //        case 169: return XLColor.Apricot;
        //        case 170: return XLColor.Aquamarine1;
        //        case 171: return XLColor.ArmyGreen;
        //        case 172: return XLColor.Arsenic;
        //        case 173: return XLColor.ArylideYellow;
        //        case 174: return XLColor.AshGrey;
        //        case 175: return XLColor.Asparagus;
        //        case 176: return XLColor.AtomicTangerine;
        //        case 177: return XLColor.Auburn;
        //        case 178: return XLColor.Aureolin;
        //        case 179: return XLColor.Aurometalsaurus;
        //        case 180: return XLColor.Awesome;
        //        case 181: return XLColor.AzureColorWheel;
        //        case 182: return XLColor.BabyBlue;
        //        case 183: return XLColor.BabyBlueEyes;
        //        case 184: return XLColor.BabyPink;
        //        case 185: return XLColor.BallBlue;
        //        case 186: return XLColor.BananaMania;
        //        case 187: return XLColor.BattleshipGrey;
        //        case 188: return XLColor.Bazaar;
        //        case 189: return XLColor.BeauBlue;
        //        case 191: return XLColor.Beaver;
        //        case 192: return XLColor.Bistre;
        //        case 193: return XLColor.Bittersweet;
        //        case 194: return XLColor.BleuDeFrance;
        //        case 195: return XLColor.BlizzardBlue;
        //        case 196: return XLColor.Blond;
        //        case 197: return XLColor.BlueBell;
        //        case 198: return XLColor.BlueGray;
        //        case 199: return XLColor.BlueGreen;
        //        case 200: return XLColor.BluePigment;
        //        case 201: return XLColor.BlueRyb;
        //        case 202: return XLColor.Blush;
        //        case 203: return XLColor.Bole;
        //        case 204: return XLColor.BondiBlue;
        //        case 205: return XLColor.BostonUniversityRed;
        //        case 206: return XLColor.BrandeisBlue;
        //        case 207: return XLColor.Brass;
        //        case 208: return XLColor.BrickRed;
        //        case 209: return XLColor.BrightCerulean;
        //        case 210: return XLColor.BrightGreen;
        //        case 211: return XLColor.BrightLavender;
        //        case 212: return XLColor.BrightMaroon;
        //        case 213: return XLColor.BrightPink;
        //        case 214: return XLColor.BrightTurquoise;
        //        case 215: return XLColor.BrightUbe;
        //        case 216: return XLColor.BrilliantLavender;
        //        case 217: return XLColor.BrilliantRose;
        //        case 218: return XLColor.BrinkPink;
        //        case 219: return XLColor.BritishRacingGreen;
        //        case 220: return XLColor.Bronze;
        //        case 221: return XLColor.BrownTraditional;
        //        case 222: return XLColor.BubbleGum;
        //        case 223: return XLColor.Bubbles;
        //        case 224: return XLColor.Buff;
        //        case 225: return XLColor.BulgarianRose;
        //        case 226: return XLColor.Burgundy;
        //        case 227: return XLColor.BurntOrange;
        //        case 228: return XLColor.BurntSienna;
        //        case 229: return XLColor.BurntUmber;
        //        case 230: return XLColor.Byzantine;
        //        case 231: return XLColor.Byzantium;
        //        case 232: return XLColor.Cadet;
        //        case 233: return XLColor.CadetGrey;
        //        case 234: return XLColor.CadmiumGreen;
        //        case 235: return XLColor.CadmiumOrange;
        //        case 236: return XLColor.CadmiumRed;
        //        case 237: return XLColor.CadmiumYellow;
        //        case 238: return XLColor.CalPolyPomonaGreen;
        //        case 239: return XLColor.CambridgeBlue;
        //        case 240: return XLColor.Camel;
        //        case 241: return XLColor.CamouflageGreen;
        //        case 242: return XLColor.CanaryYellow;
        //        case 243: return XLColor.CandyAppleRed;
        //        case 244: return XLColor.CandyPink;
        //        case 245: return XLColor.CaputMortuum;
        //        case 246: return XLColor.Cardinal;
        //        case 247: return XLColor.CaribbeanGreen;
        //        case 248: return XLColor.Carmine;
        //        case 249: return XLColor.CarminePink;
        //        case 250: return XLColor.CarmineRed;
        //        case 251: return XLColor.CarnationPink;
        //        case 252: return XLColor.Carnelian;
        //        case 253: return XLColor.CarolinaBlue;
        //        case 254: return XLColor.CarrotOrange;
        //        case 255: return XLColor.Ceil;
        //        case 256: return XLColor.Celadon;
        //        case 257: return XLColor.CelestialBlue;
        //        case 258: return XLColor.Cerise;
        //        case 259: return XLColor.CerisePink;
        //        case 260: return XLColor.Cerulean;
        //        case 261: return XLColor.CeruleanBlue;
        //        case 262: return XLColor.Chamoisee;
        //        case 263: return XLColor.Champagne;
        //        case 264: return XLColor.Charcoal;
        //        case 265: return XLColor.ChartreuseTraditional;
        //        case 266: return XLColor.CherryBlossomPink;
        //        case 267: return XLColor.Chocolate1;
        //        case 268: return XLColor.ChromeYellow;
        //        case 269: return XLColor.Cinereous;
        //        case 270: return XLColor.Cinnabar;
        //        case 271: return XLColor.Citrine;
        //        case 272: return XLColor.ClassicRose;
        //        case 273: return XLColor.Cobalt;
        //        case 274: return XLColor.ColumbiaBlue;
        //        case 275: return XLColor.CoolBlack;
        //        case 276: return XLColor.CoolGrey;
        //        case 277: return XLColor.Copper;
        //        case 278: return XLColor.CopperRose;
        //        case 279: return XLColor.Coquelicot;
        //        case 280: return XLColor.CoralPink;
        //        case 281: return XLColor.CoralRed;
        //        case 282: return XLColor.Cordovan;
        //        case 283: return XLColor.Corn;
        //        case 284: return XLColor.CornellRed;
        //        case 285: return XLColor.CosmicLatte;
        //        case 286: return XLColor.CottonCandy;
        //        case 287: return XLColor.Cream;
        //        case 288: return XLColor.CrimsonGlory;
        //        case 289: return XLColor.CyanProcess;
        //        case 290: return XLColor.Daffodil;
        //        case 291: return XLColor.Dandelion;
        //        case 292: return XLColor.DarkBrown;
        //        case 293: return XLColor.DarkByzantium;
        //        case 294: return XLColor.DarkCandyAppleRed;
        //        case 295: return XLColor.DarkCerulean;
        //        case 296: return XLColor.DarkChampagne;
        //        case 297: return XLColor.DarkChestnut;
        //        case 298: return XLColor.DarkCoral;
        //        case 299: return XLColor.DarkElectricBlue;
        //        case 300: return XLColor.DarkGreen1;
        //        case 301: return XLColor.DarkJungleGreen;
        //        case 302: return XLColor.DarkLava;
        //        case 303: return XLColor.DarkLavender;
        //        case 304: return XLColor.DarkMidnightBlue;
        //        case 305: return XLColor.DarkPastelBlue;
        //        case 306: return XLColor.DarkPastelGreen;
        //        case 307: return XLColor.DarkPastelPurple;
        //        case 308: return XLColor.DarkPastelRed;
        //        case 309: return XLColor.DarkPink;
        //        case 310: return XLColor.DarkPowderBlue;
        //        case 311: return XLColor.DarkRaspberry;
        //        case 312: return XLColor.DarkScarlet;
        //        case 313: return XLColor.DarkSienna;
        //        case 314: return XLColor.DarkSpringGreen;
        //        case 315: return XLColor.DarkTan;
        //        case 316: return XLColor.DarkTangerine;
        //        case 317: return XLColor.DarkTaupe;
        //        case 318: return XLColor.DarkTerraCotta;
        //        case 319: return XLColor.DartmouthGreen;
        //        case 320: return XLColor.DavysGrey;
        //        case 321: return XLColor.DebianRed;
        //        case 322: return XLColor.DeepCarmine;
        //        case 323: return XLColor.DeepCarminePink;
        //        case 324: return XLColor.DeepCarrotOrange;
        //        case 325: return XLColor.DeepCerise;
        //        case 326: return XLColor.DeepChampagne;
        //        case 327: return XLColor.DeepChestnut;
        //        case 328: return XLColor.DeepFuchsia;
        //        case 329: return XLColor.DeepJungleGreen;
        //        case 330: return XLColor.DeepLilac;
        //        case 331: return XLColor.DeepMagenta;
        //        case 332: return XLColor.DeepPeach;
        //        case 333: return XLColor.DeepSaffron;
        //        case 334: return XLColor.Denim;
        //        case 335: return XLColor.Desert;
        //        case 336: return XLColor.DesertSand;
        //        case 337: return XLColor.DogwoodRose;
        //        case 338: return XLColor.DollarBill;
        //        case 339: return XLColor.Drab;
        //        case 340: return XLColor.DukeBlue;
        //        case 341: return XLColor.EarthYellow;
        //        case 342: return XLColor.Ecru;
        //        case 343: return XLColor.Eggplant;
        //        case 344: return XLColor.Eggshell;
        //        case 345: return XLColor.EgyptianBlue;
        //        case 346: return XLColor.ElectricBlue;
        //        case 347: return XLColor.ElectricCrimson;
        //        case 348: return XLColor.ElectricIndigo;
        //        case 349: return XLColor.ElectricLavender;
        //        case 350: return XLColor.ElectricLime;
        //        case 351: return XLColor.ElectricPurple;
        //        case 352: return XLColor.ElectricUltramarine;
        //        case 353: return XLColor.ElectricViolet;
        //        case 354: return XLColor.Emerald;
        //        case 355: return XLColor.EtonBlue;
        //        case 356: return XLColor.Fallow;
        //        case 357: return XLColor.FaluRed;
        //        case 358: return XLColor.Fandango;
        //        case 359: return XLColor.FashionFuchsia;
        //        case 360: return XLColor.Fawn;
        //        case 361: return XLColor.Feldgrau;
        //        case 362: return XLColor.FernGreen;
        //        case 363: return XLColor.FerrariRed;
        //        case 364: return XLColor.FieldDrab;
        //        case 365: return XLColor.FireEngineRed;
        //        case 366: return XLColor.Flame;
        //        case 367: return XLColor.FlamingoPink;
        //        case 368: return XLColor.Flavescent;
        //        case 369: return XLColor.Flax;
        //        case 370: return XLColor.FluorescentOrange;
        //        case 371: return XLColor.FluorescentYellow;
        //        case 372: return XLColor.Folly;
        //        case 373: return XLColor.ForestGreenTraditional;
        //        case 374: return XLColor.FrenchBeige;
        //        case 375: return XLColor.FrenchBlue;
        //        case 376: return XLColor.FrenchLilac;
        //        case 377: return XLColor.FrenchRose;
        //        case 378: return XLColor.FuchsiaPink;
        //        case 379: return XLColor.Fulvous;
        //        case 380: return XLColor.FuzzyWuzzy;
        //        case 381: return XLColor.Gamboge;
        //        case 382: return XLColor.Ginger;
        //        case 383: return XLColor.Glaucous;
        //        case 384: return XLColor.GoldenBrown;
        //        case 385: return XLColor.GoldenPoppy;
        //        case 386: return XLColor.GoldenYellow;
        //        case 387: return XLColor.GoldMetallic;
        //        case 388: return XLColor.GrannySmithApple;
        //        case 389: return XLColor.GrayAsparagus;
        //        case 390: return XLColor.GreenPigment;
        //        case 391: return XLColor.GreenRyb;
        //        case 392: return XLColor.Grullo;
        //        case 393: return XLColor.HalayaUbe;
        //        case 394: return XLColor.HanBlue;
        //        case 395: return XLColor.HanPurple;
        //        case 396: return XLColor.HansaYellow;
        //        case 397: return XLColor.Harlequin;
        //        case 398: return XLColor.HarvardCrimson;
        //        case 399: return XLColor.HarvestGold;
        //        case 400: return XLColor.Heliotrope;
        //        case 401: return XLColor.HollywoodCerise;
        //        case 402: return XLColor.HookersGreen;
        //        case 403: return XLColor.HotMagenta;
        //        case 404: return XLColor.HunterGreen;
        //        case 405: return XLColor.Iceberg;
        //        case 406: return XLColor.Icterine;
        //        case 407: return XLColor.Inchworm;
        //        case 408: return XLColor.IndiaGreen;
        //        case 409: return XLColor.IndianYellow;
        //        case 410: return XLColor.IndigoDye;
        //        case 411: return XLColor.InternationalKleinBlue;
        //        case 412: return XLColor.InternationalOrange;
        //        case 413: return XLColor.Iris;
        //        case 414: return XLColor.Isabelline;
        //        case 415: return XLColor.IslamicGreen;
        //        case 416: return XLColor.Jade;
        //        case 417: return XLColor.Jasper;
        //        case 418: return XLColor.JazzberryJam;
        //        case 419: return XLColor.Jonquil;
        //        case 420: return XLColor.JuneBud;
        //        case 421: return XLColor.JungleGreen;
        //        case 422: return XLColor.KellyGreen;
        //        case 423: return XLColor.KhakiHtmlCssKhaki;
        //        case 424: return XLColor.LanguidLavender;
        //        case 425: return XLColor.LapisLazuli;
        //        case 426: return XLColor.LaSalleGreen;
        //        case 427: return XLColor.LaserLemon;
        //        case 428: return XLColor.Lava;
        //        case 429: return XLColor.LavenderBlue;
        //        case 430: return XLColor.LavenderFloral;
        //        case 431: return XLColor.LavenderGray;
        //        case 432: return XLColor.LavenderIndigo;
        //        case 433: return XLColor.LavenderPink;
        //        case 434: return XLColor.LavenderPurple;
        //        case 435: return XLColor.LavenderRose;
        //        case 436: return XLColor.Lemon;
        //        case 437: return XLColor.LightApricot;
        //        case 438: return XLColor.LightBrown;
        //        case 439: return XLColor.LightCarminePink;
        //        case 440: return XLColor.LightCornflowerBlue;
        //        case 441: return XLColor.LightFuchsiaPink;
        //        case 442: return XLColor.LightMauve;
        //        case 443: return XLColor.LightPastelPurple;
        //        case 444: return XLColor.LightSalmonPink;
        //        case 445: return XLColor.LightTaupe;
        //        case 446: return XLColor.LightThulianPink;
        //        case 447: return XLColor.LightYellow1;
        //        case 448: return XLColor.Lilac;
        //        case 449: return XLColor.LimeColorWheel;
        //        case 450: return XLColor.LincolnGreen;
        //        case 451: return XLColor.Liver;
        //        case 452: return XLColor.Lust;
        //        case 453: return XLColor.MacaroniAndCheese;
        //        case 454: return XLColor.MagentaDye;
        //        case 455: return XLColor.MagentaProcess;
        //        case 456: return XLColor.MagicMint;
        //        case 457: return XLColor.Magnolia;
        //        case 458: return XLColor.Mahogany;
        //        case 459: return XLColor.Maize;
        //        case 460: return XLColor.MajorelleBlue;
        //        case 461: return XLColor.Malachite;
        //        case 462: return XLColor.Manatee;
        //        case 463: return XLColor.MangoTango;
        //        case 464: return XLColor.MaroonX11;
        //        case 465: return XLColor.Mauve;
        //        case 466: return XLColor.Mauvelous;
        //        case 467: return XLColor.MauveTaupe;
        //        case 468: return XLColor.MayaBlue;
        //        case 469: return XLColor.MeatBrown;
        //        case 470: return XLColor.MediumAquamarine1;
        //        case 471: return XLColor.MediumCandyAppleRed;
        //        case 472: return XLColor.MediumCarmine;
        //        case 473: return XLColor.MediumChampagne;
        //        case 474: return XLColor.MediumElectricBlue;
        //        case 475: return XLColor.MediumJungleGreen;
        //        case 476: return XLColor.MediumPersianBlue;
        //        case 477: return XLColor.MediumRedViolet;
        //        case 478: return XLColor.MediumSpringBud;
        //        case 479: return XLColor.MediumTaupe;
        //        case 480: return XLColor.Melon;
        //        case 481: return XLColor.MidnightGreenEagleGreen;
        //        case 482: return XLColor.MikadoYellow;
        //        case 483: return XLColor.Mint;
        //        case 484: return XLColor.MintGreen;
        //        case 485: return XLColor.ModeBeige;
        //        case 486: return XLColor.MoonstoneBlue;
        //        case 487: return XLColor.MordantRed19;
        //        case 488: return XLColor.MossGreen;
        //        case 489: return XLColor.MountainMeadow;
        //        case 490: return XLColor.MountbattenPink;
        //        case 491: return XLColor.MsuGreen;
        //        case 492: return XLColor.Mulberry;
        //        case 493: return XLColor.Mustard;
        //        case 494: return XLColor.Myrtle;
        //        case 495: return XLColor.NadeshikoPink;
        //        case 496: return XLColor.NapierGreen;
        //        case 497: return XLColor.NaplesYellow;
        //        case 498: return XLColor.NeonCarrot;
        //        case 499: return XLColor.NeonFuchsia;
        //        case 500: return XLColor.NeonGreen;
        //        case 501: return XLColor.NonPhotoBlue;
        //        case 502: return XLColor.OceanBoatBlue;
        //        case 503: return XLColor.Ochre;
        //        case 504: return XLColor.OldGold;
        //        case 505: return XLColor.OldLavender;
        //        case 506: return XLColor.OldMauve;
        //        case 507: return XLColor.OldRose;
        //        case 508: return XLColor.OliveDrab7;
        //        case 509: return XLColor.Olivine;
        //        case 510: return XLColor.Onyx;
        //        case 511: return XLColor.OperaMauve;
        //        case 512: return XLColor.OrangeColorWheel;
        //        case 513: return XLColor.OrangePeel;
        //        case 514: return XLColor.OrangeRyb;
        //        case 515: return XLColor.OtterBrown;
        //        case 516: return XLColor.OuCrimsonRed;
        //        case 517: return XLColor.OuterSpace;
        //        case 518: return XLColor.OutrageousOrange;
        //        case 519: return XLColor.OxfordBlue;
        //        case 520: return XLColor.PakistanGreen;
        //        case 521: return XLColor.PalatinateBlue;
        //        case 522: return XLColor.PalatinatePurple;
        //        case 523: return XLColor.PaleAqua;
        //        case 524: return XLColor.PaleBrown;
        //        case 525: return XLColor.PaleCarmine;
        //        case 526: return XLColor.PaleCerulean;
        //        case 527: return XLColor.PaleChestnut;
        //        case 528: return XLColor.PaleCopper;
        //        case 529: return XLColor.PaleCornflowerBlue;
        //        case 530: return XLColor.PaleGold;
        //        case 531: return XLColor.PaleMagenta;
        //        case 532: return XLColor.PalePink;
        //        case 533: return XLColor.PaleRobinEggBlue;
        //        case 534: return XLColor.PaleSilver;
        //        case 535: return XLColor.PaleSpringBud;
        //        case 536: return XLColor.PaleTaupe;
        //        case 537: return XLColor.PansyPurple;
        //        case 538: return XLColor.ParisGreen;
        //        case 539: return XLColor.PastelBlue;
        //        case 540: return XLColor.PastelBrown;
        //        case 541: return XLColor.PastelGray;
        //        case 542: return XLColor.PastelGreen;
        //        case 543: return XLColor.PastelMagenta;
        //        case 544: return XLColor.PastelOrange;
        //        case 545: return XLColor.PastelPink;
        //        case 546: return XLColor.PastelPurple;
        //        case 547: return XLColor.PastelRed;
        //        case 548: return XLColor.PastelViolet;
        //        case 549: return XLColor.PastelYellow;
        //        case 550: return XLColor.PaynesGrey;
        //        case 551: return XLColor.Peach;
        //        case 552: return XLColor.PeachOrange;
        //        case 553: return XLColor.PeachYellow;
        //        case 554: return XLColor.Pear;
        //        case 555: return XLColor.Pearl;
        //        case 556: return XLColor.Peridot;
        //        case 557: return XLColor.Periwinkle;
        //        case 558: return XLColor.PersianBlue;
        //        case 559: return XLColor.PersianGreen;
        //        case 560: return XLColor.PersianIndigo;
        //        case 561: return XLColor.PersianOrange;
        //        case 562: return XLColor.PersianPink;
        //        case 563: return XLColor.PersianPlum;
        //        case 564: return XLColor.PersianRed;
        //        case 565: return XLColor.PersianRose;
        //        case 566: return XLColor.Persimmon;
        //        case 567: return XLColor.Phlox;
        //        case 568: return XLColor.PhthaloBlue;
        //        case 569: return XLColor.PhthaloGreen;
        //        case 570: return XLColor.PiggyPink;
        //        case 571: return XLColor.PineGreen;
        //        case 572: return XLColor.PinkOrange;
        //        case 573: return XLColor.PinkPearl;
        //        case 574: return XLColor.PinkSherbet;
        //        case 575: return XLColor.Pistachio;
        //        case 576: return XLColor.Platinum;
        //        case 577: return XLColor.PlumTraditional;
        //        case 578: return XLColor.PortlandOrange;
        //        case 579: return XLColor.PrincetonOrange;
        //        case 580: return XLColor.Prune;
        //        case 581: return XLColor.PrussianBlue;
        //        case 582: return XLColor.PsychedelicPurple;
        //        case 583: return XLColor.Puce;
        //        case 584: return XLColor.Pumpkin;
        //        case 585: return XLColor.PurpleHeart;
        //        case 586: return XLColor.PurpleMountainMajesty;
        //        case 587: return XLColor.PurpleMunsell;
        //        case 588: return XLColor.PurplePizzazz;
        //        case 589: return XLColor.PurpleTaupe;
        //        case 590: return XLColor.PurpleX11;
        //        case 591: return XLColor.RadicalRed;
        //        case 592: return XLColor.Raspberry;
        //        case 593: return XLColor.RaspberryGlace;
        //        case 594: return XLColor.RaspberryPink;
        //        case 595: return XLColor.RaspberryRose;
        //        case 596: return XLColor.RawUmber;
        //        case 597: return XLColor.RazzleDazzleRose;
        //        case 598: return XLColor.Razzmatazz;
        //        case 599: return XLColor.RedMunsell;
        //        case 600: return XLColor.RedNcs;
        //        case 601: return XLColor.RedPigment;
        //        case 602: return XLColor.RedRyb;
        //        case 603: return XLColor.Redwood;
        //        case 604: return XLColor.Regalia;
        //        case 605: return XLColor.RichBlack;
        //        case 606: return XLColor.RichBrilliantLavender;
        //        case 607: return XLColor.RichCarmine;
        //        case 608: return XLColor.RichElectricBlue;
        //        case 609: return XLColor.RichLavender;
        //        case 610: return XLColor.RichLilac;
        //        case 611: return XLColor.RichMaroon;
        //        case 612: return XLColor.RifleGreen;
        //        case 613: return XLColor.RobinEggBlue;
        //        case 614: return XLColor.Rose;
        //        case 615: return XLColor.RoseBonbon;
        //        case 616: return XLColor.RoseEbony;
        //        case 617: return XLColor.RoseGold;
        //        case 618: return XLColor.RoseMadder;
        //        case 619: return XLColor.RosePink;
        //        case 620: return XLColor.RoseQuartz;
        //        case 621: return XLColor.RoseTaupe;
        //        case 622: return XLColor.RoseVale;
        //        case 623: return XLColor.Rosewood;
        //        case 624: return XLColor.RossoCorsa;
        //        case 625: return XLColor.RoyalAzure;
        //        case 626: return XLColor.RoyalBlueTraditional;
        //        case 627: return XLColor.RoyalFuchsia;
        //        case 628: return XLColor.RoyalPurple;
        //        case 629: return XLColor.Ruby;
        //        case 631: return XLColor.Ruddy;
        //        case 632: return XLColor.RuddyBrown;
        //        case 633: return XLColor.RuddyPink;
        //        case 634: return XLColor.Rufous;
        //        case 635: return XLColor.Russet;
        //        case 636: return XLColor.Rust;
        //        case 637: return XLColor.SacramentoStateGreen;
        //        case 638: return XLColor.SafetyOrangeBlazeOrange;
        //        case 639: return XLColor.Saffron;
        //        case 640: return XLColor.Salmon1;
        //        case 641: return XLColor.SalmonPink;
        //        case 642: return XLColor.Sand;
        //        case 643: return XLColor.SandDune;
        //        case 644: return XLColor.Sandstorm;
        //        case 645: return XLColor.SandyTaupe;
        //        case 646: return XLColor.Sangria;
        //        case 647: return XLColor.SapGreen;
        //        case 648: return XLColor.Sapphire;
        //        case 649: return XLColor.SatinSheenGold;
        //        case 650: return XLColor.Scarlet;
        //        case 651: return XLColor.SchoolBusYellow;
        //        case 652: return XLColor.ScreaminGreen;
        //        case 653: return XLColor.SealBrown;
        //        case 654: return XLColor.SelectiveYellow;
        //        case 655: return XLColor.Sepia;
        //        case 656: return XLColor.Shadow;
        //        case 657: return XLColor.ShamrockGreen;
        //        case 658: return XLColor.ShockingPink;
        //        case 659: return XLColor.Sienna1;
        //        case 660: return XLColor.Sinopia;
        //        case 661: return XLColor.Skobeloff;
        //        case 662: return XLColor.SkyMagenta;
        //        case 663: return XLColor.SmaltDarkPowderBlue;
        //        case 664: return XLColor.SmokeyTopaz;
        //        case 665: return XLColor.SmokyBlack;
        //        case 666: return XLColor.SpiroDiscoBall;
        //        case 667: return XLColor.SplashedWhite;
        //        case 668: return XLColor.SpringBud;
        //        case 669: return XLColor.StPatricksBlue;
        //        case 670: return XLColor.StilDeGrainYellow;
        //        case 671: return XLColor.Straw;
        //        case 672: return XLColor.Sunglow;
        //        case 673: return XLColor.Sunset;
        //        case 674: return XLColor.Tangelo;
        //        case 675: return XLColor.Tangerine;
        //        case 676: return XLColor.TangerineYellow;
        //        case 677: return XLColor.Taupe;
        //        case 678: return XLColor.TaupeGray;
        //        case 679: return XLColor.TeaGreen;
        //        case 680: return XLColor.TealBlue;
        //        case 681: return XLColor.TealGreen;
        //        case 682: return XLColor.TeaRoseOrange;
        //        case 683: return XLColor.TeaRoseRose;
        //        case 684: return XLColor.TennéTawny;
        //        case 685: return XLColor.TerraCotta;
        //        case 686: return XLColor.ThulianPink;
        //        case 687: return XLColor.TickleMePink;
        //        case 688: return XLColor.TiffanyBlue;
        //        case 689: return XLColor.TigersEye;
        //        case 690: return XLColor.Timberwolf;
        //        case 691: return XLColor.TitaniumYellow;
        //        case 692: return XLColor.Toolbox;
        //        case 693: return XLColor.TractorRed;
        //        case 694: return XLColor.TropicalRainForest;
        //        case 695: return XLColor.TuftsBlue;
        //        case 696: return XLColor.Tumbleweed;
        //        case 697: return XLColor.TurkishRose;
        //        case 698: return XLColor.Turquoise1;
        //        case 699: return XLColor.TurquoiseBlue;
        //        case 700: return XLColor.TurquoiseGreen;
        //        case 701: return XLColor.TuscanRed;
        //        case 702: return XLColor.TwilightLavender;
        //        case 703: return XLColor.TyrianPurple;
        //        case 704: return XLColor.UaBlue;
        //        case 705: return XLColor.UaRed;
        //        case 706: return XLColor.Ube;
        //        case 707: return XLColor.UclaBlue;
        //        case 708: return XLColor.UclaGold;
        //        case 709: return XLColor.UfoGreen;
        //        case 710: return XLColor.Ultramarine;
        //        case 711: return XLColor.UltramarineBlue;
        //        case 712: return XLColor.UltraPink;
        //        case 713: return XLColor.Umber;
        //        case 714: return XLColor.UnitedNationsBlue;
        //        case 715: return XLColor.UnmellowYellow;
        //        case 716: return XLColor.UpForestGreen;
        //        case 717: return XLColor.UpMaroon;
        //        case 718: return XLColor.UpsdellRed;
        //        case 719: return XLColor.Urobilin;
        //        case 720: return XLColor.UscCardinal;
        //        case 721: return XLColor.UscGold;
        //        case 722: return XLColor.UtahCrimson;
        //        case 723: return XLColor.Vanilla;
        //        case 724: return XLColor.VegasGold;
        //        case 725: return XLColor.VenetianRed;
        //        case 726: return XLColor.Verdigris;
        //        case 727: return XLColor.Vermilion;
        //        case 728: return XLColor.Veronica;
        //        case 729: return XLColor.Violet1;
        //        case 730: return XLColor.VioletColorWheel;
        //        case 731: return XLColor.VioletRyb;
        //        case 732: return XLColor.Viridian;
        //        case 733: return XLColor.VividAuburn;
        //        case 734: return XLColor.VividBurgundy;
        //        case 735: return XLColor.VividCerise;
        //        case 736: return XLColor.VividTangerine;
        //        case 737: return XLColor.VividViolet;
        //        case 738: return XLColor.WarmBlack;
        //        case 739: return XLColor.Wenge;
        //        case 741: return XLColor.WildBlueYonder;
        //        case 742: return XLColor.WildStrawberry;
        //        case 743: return XLColor.WildWatermelon;
        //        case 744: return XLColor.Wisteria;
        //        case 745: return XLColor.Xanadu;
        //        case 746: return XLColor.YaleBlue;
        //        case 747: return XLColor.YellowMunsell;
        //        case 748: return XLColor.YellowNcs;
        //        case 749: return XLColor.YellowProcess;
        //        case 750: return XLColor.YellowRyb;
        //        case 751: return XLColor.Zaffre;
        //        case 752: return XLColor.ZinnwalditeBrown;
        //        case 753: return XLColor.Transparent;
        //        default:
        //            return XLColor.Black;
        //    }
        //}
    }
}
