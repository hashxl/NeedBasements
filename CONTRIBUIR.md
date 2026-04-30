Estrutura final (DDD em camadas):


NeedBasements/
├── Setup.cs                          ← 47 linhas, só ITCMod (era 312)
├── ModConstants.cs
├── Domain/
│   ├── Substances/
│   │   ├── Substance.cs              ← era SubstanceDef + lógica (SatisfactionAt, StageFor)
│   │   ├── ProgressionStage.cs
│   │   └── SubstanceCatalog.cs       ← era AllSubstances (instance, com FindByItemName)
│   └── Addiction/
│       ├── AddictionState.cs         ← encapsula timers, active substance, purchase count
│       └── CravingSchedule.cs        ← cálculo puro de intervalo
├── Application/                      ← um serviço por caso de uso
│   ├── CravingService.cs
│   ├── SubstanceUseService.cs
│   └── VendorService.cs
├── Infrastructure/                   ← integração com o jogo
│   ├── LimbEffects/ModLimbEffects.cs
│   ├── PleasureEffect.cs             ← era ApplyHeadLimbEffect
│   ├── AddictionStatFactory.cs
│   └── SubstanceItemRegistry.cs
└── Dialogues/                        ← só TEXTO (UI)
    ├── BlockedLines.cs
    ├── CravingLines.cs
    └── VendorLines.cs
Princípios aplicados:

SRP / Clean Code: cada classe um motivo de mudança. Setup.cs só orquestra; cravings, uso e vendor cada um na sua casa.
Dependências apontam pra dentro (DDD): Application depende de Domain; Infrastructure implementa detalhes de jogo; Domain não conhece ninguém.
State como agregado: AddictionState.cs centraliza _satisfactionTimer, _cravingTimer, _activeSubstance, _purchaseCount — antes espalhados em campos privados do god class.
Tell, don't ask: substance.SatisfactionAt(percent) e substance.StageFor(level) em vez de o serviço ler Stages e fazer loop.
Pragmatic — DRY/YAGNI: removi código morto (_jennaWorldPosition rastreado e nunca usado; _shopCatalogue instanciado e nunca registrado; arquivo vazio CigarProgressionLines.cs).