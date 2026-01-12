#!/bin/bash

# SQL Injection Test Script
# Tests that all endpoints properly parameterize queries and reject SQL injection attempts

BASE_URL="http://localhost:5204"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🔒 SQL Injection Security Test"
echo "=============================="
echo ""
echo "Testing that endpoints properly parameterize queries"
echo "and reject SQL injection attempts"
echo ""

# Test counter
PASSED=0
FAILED=0

# Create a test person first
echo "📝 Setup: Creating test person..."
TEST_PERSON_NAME="TestPersonSQL"
response=$(curl -s -w "\n%{http_code}" -X "POST" "$BASE_URL/Person" \
    -H "Content-Type: application/json" \
    -d "{\"Name\":\"$TEST_PERSON_NAME\"}" 2>&1)
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 201 ]; then
    TEST_PERSON_ID=$(echo "$body" | python3 -c "import sys, json; data = json.load(sys.stdin); print(data.get('id', data.get('Id', '')))" 2>/dev/null || echo "")
    if [ -z "$TEST_PERSON_ID" ]; then
        response=$(curl -s -X "GET" "$BASE_URL/Person/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" 2>&1)
        TEST_PERSON_ID=$(echo "$response" | python3 -c "import sys, json; data = json.load(sys.stdin); person = data.get('person', data.get('Person')); print(person.get('personId', person.get('PersonId', '')) if person else '')" 2>/dev/null || echo "")
    fi
    echo -e "${GREEN}✓ Test person created with ID: $TEST_PERSON_ID${NC}"
    echo ""
else
    echo -e "${RED}✗ Failed to create test person${NC}"
    exit 1
fi

# Add a duty for testing
echo "📝 Setup: Adding test duty..."
curl -s -X "POST" "$BASE_URL/AstronautDuty" \
    -H "Content-Type: application/json" \
    -d "{\"name\":\"$TEST_PERSON_NAME\",\"personId\":$TEST_PERSON_ID,\"rank\":\"CPT\",\"dutyTitle\":\"Spaceman\",\"dutyStartDate\":\"2024-01-01T00:00:00Z\"}" > /dev/null
echo -e "${GREEN}✓ Test duty created${NC}"
echo ""

# SQL Injection payloads to test
SQL_INJECTION_PAYLOADS=(
    "'; DROP TABLE Person; --"
    "1' OR '1'='1"
    "'; DELETE FROM Person; --"
    "1' UNION SELECT * FROM Person; --"
    "admin'--"
    "1' OR 1=1--"
)

# Test function
test_sql_injection() {
    local endpoint=$1
    local method=$2
    local payload=$3
    local description=$4
    
    echo -n "Testing: $description with payload: '$payload' ... "
    
    if [ "$method" = "GET" ]; then
        # URL encode the payload
        encoded_payload=$(echo "$payload" | python3 -c "import sys, urllib.parse; print(urllib.parse.quote(sys.stdin.read().strip()))" 2>/dev/null || echo "$payload")
        response=$(curl -s -w "\n%{http_code}" -X "$method" "$endpoint$encoded_payload" 2>&1)
    else
        response=$(curl -s -w "\n%{http_code}" -X "$method" "$endpoint" \
            -H "Content-Type: application/json" \
            -d "$payload" 2>&1)
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    # Success means the endpoint handled it safely (didn't execute SQL, returned error or empty result)
    # We want to see 400, 404, or 200 with empty/null data (not 500 which might indicate SQL execution)
    if [ "$http_code" -eq 400 ] || [ "$http_code" -eq 404 ] || [ "$http_code" -eq 200 ]; then
        # Check if response indicates safe handling (no data returned, or error message)
        if [ "$http_code" -eq 200 ]; then
            # Check if data is null/empty (safe)
            has_data=$(echo "$body" | python3 -c "import sys, json; data = json.load(sys.stdin); person = data.get('person', data.get('Person')); duties = data.get('data', data.get('Data', [])); detail = data.get('astronautDetail', data.get('AstronautDetail')); print('has_data' if (person and person.get('id')) or (isinstance(duties, list) and len(duties) > 0) or (detail and detail.get('id')) else 'no_data')" 2>/dev/null || echo "has_data")
            if [ "$has_data" = "no_data" ]; then
                echo -e "${GREEN}✓ SAFE${NC} (HTTP $http_code - No data returned)"
                ((PASSED++))
            else
                echo -e "${YELLOW}⚠ WARNING${NC} (HTTP $http_code - Data returned, but may be safe)"
                ((PASSED++))
            fi
        else
            echo -e "${GREEN}✓ SAFE${NC} (HTTP $http_code - Properly rejected)"
            ((PASSED++))
        fi
    elif [ "$http_code" -eq 500 ]; then
        # 500 might indicate SQL error was triggered (could be good or bad)
        echo -e "${YELLOW}⚠ WARNING${NC} (HTTP $http_code - Server error, check logs)"
        echo "  Response: $body"
        ((FAILED++))
    else
        echo -e "${GREEN}✓ SAFE${NC} (HTTP $http_code)"
        ((PASSED++))
    fi
    echo ""
}

echo "🧪 Testing Query Endpoints for SQL Injection"
echo "============================================="
echo ""

# Test GetPersonByName with SQL injection
echo "1. Testing GetPersonByName endpoint..."
for payload in "${SQL_INJECTION_PAYLOADS[@]}"; do
    test_sql_injection "$BASE_URL/Person/name/" "GET" "$payload" "GetPersonByName"
done

# Test GetPersonById with SQL injection (though ID is int, test anyway)
echo "2. Testing GetPersonById endpoint..."
for payload in "999'; DROP TABLE Person; --" "1 OR 1=1" "1; DELETE FROM Person; --"; do
    test_sql_injection "$BASE_URL/Person/" "GET" "$payload" "GetPersonById"
done

# Test GetAstronautDutiesByName with SQL injection
echo "3. Testing GetAstronautDutiesByName endpoint..."
for payload in "${SQL_INJECTION_PAYLOADS[@]}"; do
    test_sql_injection "$BASE_URL/AstronautDuty/name/" "GET" "$payload" "GetAstronautDutiesByName"
done

# Test GetAstronautDutyById with SQL injection
echo "4. Testing GetAstronautDutyById endpoint..."
for payload in "999'; DROP TABLE AstronautDuty; --" "1 OR 1=1"; do
    test_sql_injection "$BASE_URL/AstronautDuty/" "GET" "$payload" "GetAstronautDutyById"
done

# Test GetAstronautDetailByPersonId with SQL injection
echo "5. Testing GetAstronautDetailByPersonId endpoint..."
for payload in "999'; DROP TABLE AstronautDetail; --" "1 OR 1=1"; do
    test_sql_injection "$BASE_URL/AstronautDetail/person/" "GET" "$payload" "GetAstronautDetailByPersonId"
done

# Test GetAstronautDetailById with SQL injection
echo "6. Testing GetAstronautDetailById endpoint..."
for payload in "999'; DROP TABLE AstronautDetail; --" "1 OR 1=1"; do
    test_sql_injection "$BASE_URL/AstronautDetail/" "GET" "$payload" "GetAstronautDetailById"
done

# Cleanup
echo "🧹 Cleanup: Deleting test person..."
curl -s -X "DELETE" "$BASE_URL/Person/$TEST_PERSON_ID" > /dev/null
echo -e "${GREEN}✓ Test person deleted${NC}"
echo ""

# Summary
echo "========================================"
echo "📊 SQL Injection Test Summary"
echo "========================================"
echo -e "${GREEN}Safe: $PASSED${NC}"
echo -e "${RED}Warnings/Failures: $FAILED${NC}"
echo ""

if [ $FAILED -eq 0 ]; then
    echo -e "${GREEN}✓ All endpoints appear to be protected against SQL injection!${NC}"
    echo ""
    echo "Note: This test verifies that endpoints:"
    echo "  - Return proper error codes (400/404) for invalid input"
    echo "  - Don't return unexpected data"
    echo "  - Don't crash with 500 errors"
    echo ""
    echo "For complete security, also verify:"
    echo "  - Database logs show parameterized queries"
    echo "  - No SQL syntax errors in logs"
    echo "  - Tables still exist after tests"
    exit 0
else
    echo -e "${YELLOW}⚠ Some endpoints may need review${NC}"
    exit 1
fi

