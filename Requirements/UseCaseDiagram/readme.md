@startuml
left to right direction
skinparam actorStyle awesome
skinparam packageStyle rectangle
skinparam usecase {
  BorderColor #2D3E50
  BackgroundColor #EEF3F7
}
skinparam rectangle {
  BorderColor #2D3E50
}


actor "Начальник\nбудівельного майданчика" as Supervisor
actor "Інженер з\nтехніки безпеки" as Safety
actor "Системний\nадміністратор" as Admin
actor "Керівництво\nкомпанії" as Management


rectangle "FallingDetectionService" as System {
  usecase "(UC-1.2)\nОтримати сповіщення\nпро падіння" as UC12
  usecase "(UC-2.2)\nСформувати звіт\nпро безпеку" as UC22
  usecase "(UC-3.1)\nЗареєструвати\nIoT-пристрій" as UC31
}


Supervisor -- UC12
Safety -- UC22
Admin -- UC31
Management -- UC22 : Отримує зведені звіти


@enduml
