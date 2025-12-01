@startuml
skinparam rectangle {
  BackgroundColor #EEF3F7
  BorderColor #2D3E50
}
skinparam shadowing false

title Горизонтальний прототип: Головна панель моніторингу

rectangle "FallingDetectionService" {
  rectangle " \n- Назва системи\n- Ім'я користувача (Віктор)\n- Кнопка виходу" as Header
  rectangle " \n- Моніторинг\n- Інциденти\n- Звіти\n- IoT-пристрої\n- Налаштування" as Nav
  rectangle " \n- Карта будмайданчика\n- Маркери інцидентів\n- Статус зон (зелена/жовта/червона)" as Map
  rectangle " \n- Список останніх інцидентів\n- Статус тривог\n- Швидкі дії" as RightPanel
}
@enduml


@startuml
skinparam rectangle {
  BackgroundColor #F5F7FA
  BorderColor #2D3E50
}
skinparam shadowing false

title Горизонтальний прототип: Екран деталей інциденту

rectangle "Екран деталей інциденту" {
  rectangle " \n- Назад до списку\n- ID інциденту\n- Статус (Нове/Підтверджено/Хибна тривога)" as H

  rectangle " \n- Дата/час події\n- Локація (координати + зона)\n- Працівник / Пристрій\n- Тип: падіння / ризик падіння" as Info

  rectangle " \n- Міні-карта з місцем інциденту\n- Таймлайн подій\n- Посилання на відео (якщо є)" as Center

  rectangle " \n- Кнопки: Підтвердити, Хибна тривога\n- Поле для коментаря\n- Журнал дій (хто що зробив і коли)" as Actions
}
@enduml




@startuml
skinparam rectangle {
  BackgroundColor #FFFFFF
  BorderColor #2D3E50
}
skinparam shadowing false

title Горизонтальний прототип: Екран реєстрації IoT-пристрою

rectangle "Екран реєстрації IoT-пристрою" {
  rectangle " \n- Назад до списку пристроїв\n- Назва: Новий IoT-пристрій" as Head

  rectangle "Форма реєстрації:" as Form {
    rectangle "Поле: ID пристрою" as f1
    rectangle "Поле: IP-адреса / адреса шлюзу" as f2
    rectangle "Поле: Тип пристрою (сенсор, камера, трекер)" as f3
    rectangle "Поле: Сертифікат безпеки (завантажити файл)" as f4
    rectangle "Випадаючий список: Призначена зона майданчика" as f5
  }

  rectangle "Кнопки дій:\n- Перевірити з’єднання\n- Зберегти\n- Скасувати" as Buttons

  rectangle "Область повідомлень:\n- Статус перевірки\n- Помилки безпеки\n- Підказки адміністратору" as Messages
}
@enduml
