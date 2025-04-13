using System;
using System.Collections.Generic;
using System.Text;

namespace MurderMystery.Enums
{
    public enum Footwear
    {
        // Formal/Professional
        DressShoe,       // Formal shoes with smooth soles and distinctive heel marks (Lawyer, Diplomat)
        OxfordShoe,      // Classic lace-up with traditional sole pattern (Butler, Professor) 
        BrogueShoe,      // Distinctive decorative perforations in the impression (Wealthy Industrialist)
        LoaferShoe,      // Slip-on dress shoes with characteristic sole pattern (Art Dealer, Card Sharp)
        WingtipShoe,     // Formal shoes with decorative toe cap (Family Lawyer, Chess Grandmaster)
        PatentLeather,   // Highly polished formal shoes (Opera Singer, Ballroom Dancer)

        // Women's Shoes
        HighHeel,        // Women's shoes with narrow, deep heel impressions (Socialite, Stage Actress)
        Pump,            // Classic women's heel with pointed toe (Fashion Designer, Society Photographer)
        KittenHeel,      // Low, slender heel (Librarian, Medium/Spiritualist)
        BalletFlat,      // Flat women's shoe (Housekeeper, Opera Singer)

        // Casual/Specialty
        SportShoe,       // Athletic shoes with treaded patterns and brand-specific designs (Racing Driver)
        Sneaker,         // Casual athletic shoes with brand-specific patterns (Jazz Musician)
        WalkingShoe,     // Comfortable shoes with moderate tread pattern (Family Doctor, Archaeologist)
        Sandal,          // Open footwear with strap patterns (Botanist, Yacht Captain)
        Slipper,         // Indoor footwear with minimal tread pattern (Retired occupations)
        DeckShoe,        // Non-marking rubber soles (Yacht Captain)
        DancingShoe,     // Specialized smooth soles (Ballroom Dancer)

        // Boots
        Boot,            // Work or hiking boots with deep treads and defined edges (Gardener)
        Wellington,      // Rain boots with smooth, wide impressions (Gardener, Botanist)
        MilitaryBoot,    // Combat boots with aggressive tread patterns (Retired Colonel)
        RidingBoot,      // Distinctive heel and smooth shaft impressions (Horse Trainer)
        HikingBoot,      // Deep lugged tread pattern (Botanical Explorer, Big Game Hunter)
        WorkBoot,        // Heavy-duty boots with safety toe impressions (Chef)
        LaceupBoot,      // Fashion boots with varied patterns (Fashion Designer)
        HuntingBoot,     // Waterproof boots with camouflage pattern impressions (Big Game Hunter)
        ChefClog,        // Kitchen-specific non-slip shoes (Chef)

        // Other
        Barefoot,        // Human foot impressions with toe marks
        CricketSpike,    // Sports shoes with distinctive spike marks (Cricket Captain)
        FencingShoe,     // Specialized athletic footwear with smooth gliding surface (Athletes)
        JodhpurBoot,     // Ankle-high riding boot (Horse Trainer)
        Unknown          // Unidentifiable footwear impression (Jewel Thief might purposely use these)
    }
}
