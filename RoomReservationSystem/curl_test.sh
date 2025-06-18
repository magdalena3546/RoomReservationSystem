#!/usr/bin/env bash
# ----------------------------------------------
# 1. Logs in with the admin user
# 2. Copies the JWT from the response
# 3. Performs two protected GET calls (Rooms, Reservations)
# ----------------------------------------------

API_BASE="https://localhost:7016"   # or http://localhost:5196 if you prefer HTTP
LOGIN_PAYLOAD='{"email":"admin@example.com","password":"admin123"}'

echo "=== 1. LOGIN ==="
echo

LOGIN_RESPONSE=$(curl -sk -X POST "$API_BASE/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d "$LOGIN_PAYLOAD")

echo "$LOGIN_RESPONSE"
echo
echo "➡  Copy the value of \"token\" from the JSON above."
read -rp "Paste JWT token here: " TOKEN
echo

echo "=== 2. GET /api/Room ==="
curl -sk -X GET "$API_BASE/api/Room" \
  -H "Authorization: Bearer $TOKEN"
echo
echo

echo "=== 3. GET /api/Reservation ==="
curl -sk -X GET "$API_BASE/api/Reservation" \
  -H "Authorization: Bearer $TOKEN"
echo
echo "=== DONE ==="
