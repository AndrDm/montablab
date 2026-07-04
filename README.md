# montab

Боковая панель-таскбар для Windows с **постоянными живыми превью** всех открытых
окон. Док слева или справа, резервирует рабочую область (развёрнутые окна не
перекрывают панель), обновление превью — в реальном времени через DWM-композитор,
практически бесплатно по ресурсам.

Один exe **~2 МБ** (NativeAOT, без зависимостей), ~15 МБ RAM, ~0% CPU в простое.

## Возможности

- Живые превью всех окон (всех мониторов) с сохранением аспекта; новые — сверху.
- Свёрнутые и «погашенные» окна — компактные полоски с иконкой и названием.
- Активное окно подсвечено рамкой, его превью затенено.
- Виртуализация: превью за пределами видимости не потребляют ресурсы.

## Управление

| Действие | Результат |
|---|---|
| Клик по превью/полоске | Переключение в окно |
| Двойной клик по живому превью (в любом месте) | Свернуть окно (системный minimize) в полоску |
| Клик по ✕ в заголовке | Закрыть приложение |
| Колесо мыши | Прокрутка ленты |
| Наведение на превью (~0,7 с) | Временная лупа ×5, движение мыши панорамирует; уход с превью — возврат |
| Ctrl + колесо над превью | Постоянный zoom ×1–5 |
| Ctrl + движение мыши | Панорамирование увеличенного превью |
| Ctrl + клик | Сброс zoom/pan |
| Перетаскивание превью | Изменение порядка |
| Перетаскивание за «ручку» сверху | Перенос панели на другой монитор/край |
| Перетаскивание внутреннего края | Ширина панели (3–20% ширины монитора) |
| Правый клик | Меню: край дока, автозапуск, выход |

## Браузер замирает в превью?

Chromium-браузеры (Chrome, Brave, Edge) отслеживают перекрытие своего окна
(«native window occlusion»): как только окно полностью закрыто другими,
браузер перестаёт рендерить кадры — звук играет, а превью в панели замирает
на последнем кадре. Это не ограничение montab: DWM показывает только то, что
приложение само отрисовало.

Лечится штатной политикой браузера — выполните в PowerShell **одну** команду
под свой браузер и перезапустите его:

```powershell
# Brave
New-Item 'HKCU:\Software\Policies\BraveSoftware\Brave' -Force |
  Set-ItemProperty -Name NativeWindowOcclusionEnabled -Value 0 -Type DWord

# Chrome
New-Item 'HKCU:\Software\Policies\Google\Chrome' -Force |
  Set-ItemProperty -Name NativeWindowOcclusionEnabled -Value 0 -Type DWord

# Edge
New-Item 'HKCU:\Software\Policies\Microsoft\Edge' -Force |
  Set-ItemProperty -Name NativeWindowOcclusionEnabled -Value 0 -Type DWord
```

Проверить, что политика применилась: `brave://policy` (или `chrome://policy`,
`edge://policy`) — там должна появиться `NativeWindowOcclusionEnabled: 0`.
Откат — удалить значение:
`Remove-ItemProperty 'HKCU:\Software\Policies\BraveSoftware\Brave' -Name NativeWindowOcclusionEnabled`.

Альтернатива без реестра — ключи в ярлыке браузера:
`--disable-features=CalculateNativeWinOcclusion --disable-backgrounding-occluded-windows`.

Цена: полностью перекрытый браузер продолжает тратить GPU/CPU на отрисовку.
На свёрнутые окна не влияет (они в панели и так полоски).

## Сборка

Требуется .NET 11 SDK (сейчас — preview 5+) и MSVC (для NativeAOT-линковки). Windows 10 1809+.

```powershell
dotnet build                     # dev-сборка
dotnet publish -c Release -r win-x64   # один exe ~2 МБ
```

Если линковка падает с ошибкой про `vswhere.exe`, добавьте в PATH:
`C:\Program Files (x86)\Microsoft Visual Studio\Installer`.

## Технологии

Чистый Win32 (без WPF/WinUI): DWM Thumbnail API (превью), AppBar API
(резервирование рабочей области), SetWinEventHook (трекинг окон без поллинга),
GDI (отрисовка), CsWin32 (P/Invoke source generator), Per-Monitor V2 DPI.
Подробности — в [PLAN.md](PLAN.md).
