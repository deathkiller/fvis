<h1 align="center">
    Detail Function Visualization
</h1>

<div align="center">
    Detailní vizualizace průběhu funkce
</div>

<div align="center">
  <sub>
    Brought to you by <a href="https://github.com/deathkiller">@deathkiller</a>
  </sub>
</div>
<hr/>

[![Build Status](https://img.shields.io/appveyor/ci/deathkiller/fvis/master.svg?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZmlsbD0iI2ZmZmZmZiIgZD0iTTI0IDIuNXYxOUwxOCAyNCAwIDE4LjV2LS41NjFsMTggMS41NDVWMHpNMSAxMy4xMTFMNC4zODUgMTAgMSA2Ljg4OWwxLjQxOC0uODI3TDUuODUzIDguNjUgMTIgM2wzIDEuNDU2djExLjA4OEwxMiAxN2wtNi4xNDctNS42NS0zLjQzNCAyLjU4OXpNNy42NDQgMTBMMTIgMTMuMjgzVjYuNzE3eiI+PC9wYXRoPjwvc3ZnPg==)](https://ci.appveyor.com/project/deathkiller/fvis)
[![Tests](https://img.shields.io/appveyor/tests/deathkiller/fvis.svg?compact_message)](https://ci.appveyor.com/project/deathkiller/fvis/build/tests)
[![Coverage](https://img.shields.io/codecov/c/github/deathkiller/fvis.svg)](https://codecov.io/gh/deathkiller/fvis)
[![Code Quality](https://img.shields.io/codacy/grade/5eac2aa64dfa4e91ab8c3144e290783a.svg)](https://www.codacy.com/app/deathkiller/fvis)
[![License](https://img.shields.io/github/license/deathkiller/fvis.svg)](https://github.com/deathkiller/fvis/blob/master/LICENSE)
[![Lines of Code](https://img.shields.io/badge/lines%20of%20code-18k-blue.svg)](https://github.com/deathkiller/fvis/graphs/code-frequency)


The purpose of this thesis is to visually investigate precision of various implementations of mathematic functions from math.h. This thesis deals with parsing of arithmetic expressions and its visualization.

The application allows visualizing even complicated arithmetic expressions. It can also compare results of the same arithmetic expressions using different implementation of math.h for computation.

Arithmetic expressions are parsed using the shunting-yard algorithm. In addition, the visualization uses MPIR library for high-precision computation. The application is written in C# programming language and uses .NET Framework.


#### *In Czech*
Tato práce se zabývá porovnáváním přesností různých implementací matematických funkcí math.h, zpracováním aritmetických výrazů a jejich vizualizací.

Vytvořená aplikace umožňuje vizualizovat i složitější matematické funkce. Dále umožnuje porovnávat výsledky stejných funkcí, které jsou spočítané různými algoritmy.

Aritmetické výrazy jsou zpracovány pomocí algoritmu shunting-yard. Vizualizace využívá knihovnu MPIR pro výpočty s vysokou přesností. Aplikace je napsaná v jazyce C# a využívá technologii .NET Framework.


## License
This project is licensed under the terms of the [GNU General Public License v3.0](./LICENSE).

Uses [Mpir.NET](http://wezeku.github.io/Mpir.NET/) licensed under the terms of the [GNU Lesser General Public License v3.0](https://github.com/wezeku/Mpir.NET/blob/master/LICENSE.txt).

Uses [LegacyWrapper](https://github.com/CodefoundryDE/LegacyWrapper) licensed under the terms of the [MIT License](https://github.com/CodefoundryDE/LegacyWrapper/blob/master/LICENSE).

Uses [TxTranslation](https://github.com/ygoe/TxTranslation) licensed under the terms of the [GNU Lesser General Public License v3.0](https://github.com/ygoe/TxTranslation/blob/master/LICENSE-LGPL).

Some icons by [Yusuke Kamiyamane](http://p.yusukekamiyamane.com/), licensed under a [Creative Commons Attribution 3.0 License](http://creativecommons.org/licenses/by/3.0/).