# RoR2-ForceItemsEqualShare
RoR2 mod - force all players to have similar amount of items per match

Mod will prevent players from picking items if they have significantly more items than the player with the fewest items.

### Current options to deal with items disproportion:

#### - Add random (mostly low tier) item to players who are behind (have low inventory cost)

#### - Block picking item for player with most inventory cost (allows for ping + interaction sharing)

### How it works
Let's say a player tries to pick an item. The mod calculates the total costs of this player's inventory and finds the player with the lowest total inventory costs.
Now we take the difference between the costs of these inventory and see if it exceeds the threshold. If so, mod will try to deal with item disproportion.

#### Inventory Costs
Inventory costs are calculated by taking each item from the player's inventory, multiplying it by its tier cost, and then adding it to the rest of the inventory cost.

#### Threshold
Threshold is calulcated by taking summed cost of inventory of player who is trying to pick item, multiplying it by **ScaleItemsCostsDifference**, and then checking whenever it falls between **MinItemsCostsDifference** and **MaxItemsCostsDifference**. If it does, then calulcated number is taken as thereshold, if not then min or max value are taken.

#### Share item (only if block option is selected)
If player can't pick item, they can ping it and use interaction button (E on keyboard by default) to share it with player with lowest inventory costs

### Default Config Settings
| Setting                       | Default Value                     |
| :---------------------------- | :-------------------------------: |
| HowToHandleItemDisproportion  | GiveRandomItemToLowestCostsPlayer |
| ScaleItemsCostsDifference     |           0.3                     |
| MinItemsCostsDifference       |             5                     |
| MaxItemsCostsDifference       |            15                     |
| WhiteItemsCost                |             1                     |
| GreenItemsCost                |             2                     |
| RedItemsCost                  |             4                     |
| BossItemsCost                 |             2                     |
| BlueItemsCost                 |             0                     |

### Additional infos

#### Mod is ignored on maps:
- Bazzar
- Void
- Moon
- Moon2

#### Items ignored in calulcation:
- ArtifactKey
- ExtraLifeConsumed
- TitanGoldDuringTP
- TonicAffliction
- CaptainDefenseMatrix

#### Items ignored while picking up:
- ArtifactKey
- ExtraLifeConsumed
- TitanGoldDuringTP
- TonicAffliction
- CaptainDefenseMatrix
- ScrapWhite
- ScrapGreen
- ScrapRed
- ScrapYellow

## More

Find my other mods here: https://thunderstore.io/package/Mordrog/

### Changelog
#### 1.3.0
- Fix for Anniversary Update
- Added new Moon stage to ignored Stages for item distribution
- Added new configuration to handle item disproportion (give low tier item to player with least inventory cost)

#### 1.2.0
- Added abbility to share item with player who have lowest inventory costs via ping + interaction
- Rebalanced default config after adding above option
- Added Moon to ignored map list
#### 1.1.0
- Rebalanced default config values to allow better players to get more items late game
- Added chat messages for players who tries to pick item when they have too many of them

