
### Задача:

Вася путешествует с обходным листом по отделам очень забюрократизированной организации и ставит печати в обходном листе.

Организация состоит из N отделов пронумерованных числами от 1 до N.
Организация использует набор из M видов печатей пронумерованных числами от 1 до M.

N, M - натуральные числа больше 1.

Вася начинает путешествие в отделе A с пустым обходным листом и заканчивает, когда приходит в отдел Z, где A и Z - заданные конфигурацией идентификаторы отделов от 1 до N.

Печати бывают разных видов. В обходном листе может быть только одна незачеркнутая печать одного вида. Если требуется поставить печать и она уже есть в обходном листе, то еще одна такая же печать не ставится. Если печать зачеркнута, то считается что ее нет и можно поставить новую такую же.

Состоянием обходного листа считается набор идентификаторов незачеркнутых печатей.

Каждый отдел имеет одно правило одного из двух видов:

**Безусловное:**
1. Поставить новую печать I если ее еще нет (или она зачеркнута) или не ставить никакую.
2. Зачеркнуть существующую печать J если она уже есть и незачеркнута или не зачеркивать никакую.
3. Отправить Васю в следующий отдел K.

**Условное:**
Если в обходном листе есть незачеркнутая печать S, то:
1. Поставить новую печать I если ее еще нет (или она зачеркнута) или не ставить никакую.
2. Зачеркнуть существующую печать J если она уже есть и незачеркнута или не зачеркивать никакую.
3. Отправить Васю в следующий отдел K.
   Иначе:
1. Поставить новую печать T если ее еще нет (или она зачеркнута) или не ставить никакую.
2. Зачеркнуть существующую печать R если она уже есть (или незачеркнута) или не зачеркивать никакую.
3. Отправить Васю в следующий отдел P.
   Здесь S, I, J, T, R - идентификаторы печатей от 1 до M. K и P - идентификаторы отделов от 1 до N.

Вася перемещается от одного отдела к другому выполняя правила установленные для этих отделов.

Возможны конфигурации, в которых Вася посещает один и тот же отдел несколько раз.

Возможны конфигурации, в которых Вася застревает в бесконечном цикле и никогда не приходит в отдел Z.

Возможны конфигурации, в которых Вася посещает не все отделы.

### Нужно написать библиотеку на C#, которая позволит:

Задать конфигурацию отделов с помощью какого-то удобного API.
Отвечать на запросы вида "какие незачеркнутые печати есть в обходном листе Васи, когда он в ходе путешествия выходит из отдела Q" для заданной конфигурации. Q - любой отдел из заданных. Если Вася в ходе путешествия посещает отдел Q несколько раз, то нужно вернуть все различающиеся варианты обходного листа. Если Вася попал в бесконечный цикл, то так же нужно вернуть все различающиеся варианты обходного листа и сообщить о наличии бесконечного цикла. Если Вася никогда не посещает отдел Q об этом тоже нужно как-то сообщить.
Библиотека должна быть покрыта тестами. Желательно, чтобы были тесты, тестирующие сценарии конкурентных запросов из нескольких потоков.

### Немного о решении
Основной класс это `DismissalService` у него есть метод `DismissalService.GetStamps` который выполняет поставленную задачу. Этот метод возвращает обьект типа `GetStampsResult`, который хранит в себе информацию о том сколько раз был посещен отдел Q, попал ли Василий в бесконечный цикл, какие различные варианты обходного листа будут у Василия в процессе обхода отделов и т.д.

Класс `Organization.Builder`  поможет создать организацию, примеры его использования можно посмотреть в тестах `GetStampsTestCases`

Класс `StampsCollection` представляет собой обходный лист с печатями

Еще внимания заслуживает класс `InfinityCycleChecker` и его интерфейс `IInfinityCycleChecker`. Как мне кажется, могут быть разные стратегии проверки на зацикливание и оптимальная стратегия может зависить от структуры графа(организации). Например, если в организации отделы только с безусловными правилами, то для проверки на закцикливание достаточно лишь запоминать в каких отделах Василий уже был. 
В моей реализации этого интерфейса, при попадании на обратное ребро(которое ведет к отделу, в котором Вася уже был), я запоманию состояние обходного листа и при повторном попадании на это ребро проверяю: если обходной лист такой же как и был раньше, то Вася попал... в бесконечный цикл. Хотел реализовать еще такой вариант: вести историю переходов Васи и при попадании на обратное ребро восстанавливать, с помощью истории, состояние обходного листа в прошлом попадании, но время поджимало :)
