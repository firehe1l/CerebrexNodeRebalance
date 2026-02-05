# Cerebrex Node Rebalance - Archotech Submission Scenario

Мод для RimWorld 1.6, добавляющий механику подчинения Архотеку через квесты и ретранслятор.

## Возможности

### ⚡ Улучшенный Cerebrex Node
- **Mechanoid Bandwidth**: +30 (было +15)
- **Mechanoid Work Speed**: +50% (было +12%)
- **Orbital Mechanoid Strike**: орбитальная бомбардировка (cooldown 1 день)
- **Mechanoid Supply Drop**: направляемый сброс ресурсов (range 45, выбор: Steel/Plasteel/Components/Adv. Components/Medicine)

### 🌌 Квестовая линия Архотеков
1. **Триггер**: Надеть Cerebrex Node → получить квест "Relay Coordinates"
2. **Квест**: Зачистить Ancient Complex, найти Signal Amplifier
3. **Постройка**: Построить Archotech Relay (требует Signal Amplifier)
4. **Контакт**: Взаимодействие с Relay открывает скрытую фракцию Archotechs

### 🤖 Нейтрализация механоидов
- После подчинения Mech Hive фракция Mechanoid становится нейтральной

## Установка

### Требования
- RimWorld 1.6
- DLC: Biotech, Odyssey
- Mod: [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)

### Установка из исходников

1. **Клонировать репозиторий**:
   ```bash
   git clone <repo-url>
   cd rebalance
   ```

2. **Собрать DLL** (выберите один из вариантов):

   **Вариант A: .NET SDK**
   ```powershell
   cd Source
   dotnet build -c Release
   ```
   
   **Вариант B: Visual Studio**
   - Открыть `Source/CerebrexRebalance.csproj`
   - Build → Build Solution (Release)
   
   **Вариант C: MSBuild**
   ```powershell
   cd Source
   "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" CerebrexRebalance.csproj /p:Configuration=Release
   ```

3. **Скопировать в RimWorld Mods**:
   ```
   C:\Users\<User>\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods\CerebrexRebalance\
   ```
   Или симлинк:
   ```powershell
   New-Item -ItemType SymbolicLink -Path "C:\Users\<User>\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods\CerebrexRebalance" -Target "c:\Users\user\Desktop\Code Projects\rebalance"
   ```

4. **Активировать мод** в RimWorld Mod Manager

## Совместимость

✅ **Combat Extended** - полностью совместим (патчи загружаются автоматически)  
✅ **Vanilla Expanded Framework** - совместим  
✅ **Мультиплеер** - safe (детерминированный RNG, нет DateTime.Now)

## Структура файлов

```
rebalance/
├── About/
│   └── About.xml                  # Метаданные мода
├── Assemblies/
│   └── CerebrexRebalance.dll     # Скомпилированный код (после сборки)
├── Defs/
│   ├── Abilities_OrbitalStrike.xml
│   ├── Abilities_SupplyDrop.xml
│   ├── Buildings/Relay.xml
│   ├── Factions/ArchotechFaction.xml
│   ├── Items/SignalAmplifier.xml
│   ├── Quests/ArchotechQuest.xml
│   └── WorldObjects/ArchotechSite.xml
├── Patches/
│   └── CerebrexNode_Patches.xml
├── CombatExtended/
│   └── Patches/
│       └── CE_OrbitalStrike_Patch.xml
├── LoadFolders.xml
└── Source/
    ├── ArchotechQuestGiver.cs
    ├── CerebrexDefOf.cs
    ├── CerebrexRebalanceInit.cs
    ├── FactionNeutralizer.cs
    ├── GenStep_SpawnSignalAmplifier.cs
    ├── OrbitalStrikeAbility.cs
    ├── RelayInteraction.cs
    └── SupplyDropAbility.cs
```

## Игровой процесс

### 1. Получение квеста
- Наденьте Cerebrex Node на любого колониста
- Автоматически появится квест "Relay Coordinates"

### 2. Прохождение квеста
- Отправьтесь на координаты (Ancient Complex)
- Зачистите механоидов
- Найдите **Signal Amplifier** (зелёный AI Persona Core)

### 3. Постройка Relay
- Требуется: 200 Steel, 50 Components, 10 Adv. Components, **1 Signal Amplifier**
- Потребление: 500W
- Категория: Misc

### 4. Контакт с Архотеками
- Только колонисты с **Cerebrex Node** могут использовать Relay
- ПКМ на Relay → "Call Archotech"
- Фракция "Archotechs" открывается

## Способности

### Mechanoid Supply Drop (улучшенная)
- **Range**: 45 tiles
- **Cooldown**: 2-3 дня
- **Targeting**: клик для выбора точки
- **Ресурсы**: Steel (350), Plasteel (100), Components (20), Adv. Components (5), Medicine (25)

### Orbital Mechanoid Strike (новая)
- **Range**: 45 tiles
- **Cooldown**: 1 день (60000 ticks)
- **Эффект**: серия взрывов (8 взрывов, радиус 4.9)
- **Ограничения**: не работает под толстой крышей

## Разработка

### Оптимизация
- ✅ DefOf pattern для всех Def (кеширование)
- ✅ Null-checks на всех уровнях
- ✅ Try-catch для критических операций
- ✅ Нет тяжёлых операций в Tick()

### Тестирование
Dev Mode команды:
```
godmode              # мгновенное строительство
!items               # спавн предметов
!quests              # генерация квестов
```

## Лицензия

MIT License (см. LICENSE)

## Контрибьют

Pull requests приветствуются!

## Известные проблемы

- GenStep может спавнить Signal Amplifier не в самой очевидной комнате (ищите около центра карты)
- Квестовая карта использует стандартный Ancient Complex (не кастомный механоид-биом)

## Планы на будущее

- [ ] Кастомные текстуры для Relay и Signal Amplifier
- [ ] Торговля с фракцией Архотеков
- [ ] Квесты от фракции
- [ ] Кастомный механоид-биом для квестовых карт
