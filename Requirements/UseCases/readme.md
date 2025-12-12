



### UC-SIMPLE-1 — Прийом інциденту
- **Опис:** Приймаємо інцидент від зовнішнього конектора і зберігаємо його.
- **Тригер:** `POST /api/v1/incidents`
- **Обов'язкові поля у payload:** `siteId`, `zoneId`, `timestamp`, `type`, `confidence`, `sourceId`, `evidenceRef`.
- **Потік:**
  1. Отримуємо `POST /api/v1/incidents` з payload.
  2. Валідуємо мінімальні поля.
  3. Зберігаємо запис `Incident` зі статусом `NEW`.
  4. Якщо `confidence` достатньо високе (просте порівняння з числом у коді), створюємо запис `Alert` (без надсилання зовнішніх повідомлень).
  5. Повертаємо `201 Created` з `incidentId`.
- **Винятки:** Невалідний payload → `400`; внутрішня помилка → `5xx`.

Suggested endpoints:
- `POST /api/v1/incidents`
- `GET /api/v1/incidents/{id}`

---

### UC-SIMPLE-2 — Генерація звіту
- **Опис:** Генеруємо простий звіт по інцидентах за період і повертаємо CSV або JSON.
- **Тригер:** `GET /api/v1/reports/safety?siteId=&start=&end=&format=csv`
- **Потік:**
  1. Запит з параметрами періоду і `siteId`.
  2. Система виконує просту агрегацію та повертає результат у вказаному форматі.
  3. Запит обробляється синхронно (без черг).
- **Винятки:** Нема даних → порожній файл/`204`.

Suggested endpoint:
- `GET /api/v1/reports/safety`

---

### UC-SIMPLE-3 — Додавання джерела (device)
- **Опис:** Додаємо запис джерела, яке надсилатиме інциденти. Ніяких credential-операцій.
- **Тригер:** `POST /api/v1/devices`
- **Потік:**
  1. Адмін або інженер викликає `POST /api/v1/devices` з `sourceId`, `type`, `siteId` та метаданими.
  2. Система зберігає запис і повертає `201 Created`.
  3. Після цього джерело може почати надсилати `POST /api/v1/incidents`.
- **Винятки:** `sourceId` вже існує → `409`.

Suggested endpoint:
- `POST /api/v1/devices`

---



