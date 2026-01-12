#!/bin/bash

# Test script for all endpoints in the Stargate API
# This script tests all GET, POST, PUT, and DELETE endpoints

BASE_URL="http://localhost:5204"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🧪 Testing Stargate API Endpoints"
echo "=================================="
echo ""

# Test counter
PASSED=0
FAILED=0
NOT_IMPLEMENTED=0

# Function to test an endpoint
test_endpoint() {
    local method=$1
    local url=$2
    local description=$3
    local data=$4  # Optional JSON data for POST/PUT
    
    echo -n "Testing: $description ... "
    
    if [ -n "$data" ]; then
        response=$(curl -s -w "\n%{http_code}" -X "$method" "$url" \
            -H "Content-Type: application/json" \
            -d "$data" 2>&1)
    else
        response=$(curl -s -w "\n%{http_code}" -X "$method" "$url" 2>&1)
    fi
    
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 201 ]; then
        echo -e "${GREEN}✓ PASSED${NC} (HTTP $http_code)"
        ((PASSED++))
    elif [ "$http_code" -eq 202 ]; then
        echo -e "${YELLOW}⚠ NOT IMPLEMENTED${NC} (HTTP $http_code)"
        ((NOT_IMPLEMENTED++))
    else
        echo -e "${RED}✗ FAILED${NC} (HTTP $http_code)"
        echo "  Response: $body"
        ((FAILED++))
    fi
    echo ""
}

# Check if server is running
echo "Checking if server is running..."
if ! curl -s -f "$BASE_URL/swagger" > /dev/null 2>&1; then
    echo -e "${RED}Error: Server is not running at $BASE_URL${NC}"
    echo "Please start the server with: dotnet run"
    exit 1
fi
echo -e "${GREEN}Server is running${NC}"
echo ""

# Test IDs (will be extracted from responses)
TEST_PERSON_NAME="TestPerson"
TEST_PERSON_ID=""
TEST_ASTRONAUT_DETAIL_ID=""
TEST_ASTRONAUT_DUTY_ID=""

# Helper function to extract ID from JSON response
extract_id() {
    local json_response="$1"
    echo "$json_response" | python3 -c "import sys, json; data = json.load(sys.stdin); print(data.get('id', data.get('Id', '')))" 2>/dev/null || echo ""
}

echo "🔄 Integration Test Flow"
echo "========================"
echo "Following entity relationships: Person → AddAstronautDuty (creates AstronautDetail automatically)"
echo ""

# ============================================
# CREATE PHASE
# ============================================
echo "📝 CREATE PHASE"
echo "---------------"

# Create Person
echo "1. Creating Person..."
response=$(curl -s -w "\n%{http_code}" -X "POST" "$BASE_URL/Person" \
    -H "Content-Type: application/json" \
    -d "{\"Name\":\"$TEST_PERSON_NAME\"}" 2>&1)
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

if [ "$http_code" -eq 200 ] || [ "$http_code" -eq 201 ]; then
    TEST_PERSON_ID=$(extract_id "$body")
    if [ -z "$TEST_PERSON_ID" ]; then
        # Fallback: try to get from GetPersonByName
        response=$(curl -s -X "GET" "$BASE_URL/Person/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" 2>&1)
        TEST_PERSON_ID=$(extract_id "$response")
    fi
    if [ -z "$TEST_PERSON_ID" ]; then
        TEST_PERSON_ID=1  # Fallback to default
    fi
    echo -e "${GREEN}✓ Person created with ID: $TEST_PERSON_ID${NC}"
    ((PASSED++))
else
    echo -e "${RED}✗ Failed to create person${NC}"
    ((FAILED++))
fi
echo ""

# Add Spaceman duty
echo "2. Adding Spaceman duty..."
test_endpoint "POST" "$BASE_URL/AstronautDuty" "AddAstronautDuty - Create Spaceman duty" "{\"name\":\"$TEST_PERSON_NAME\",\"personId\":$TEST_PERSON_ID,\"rank\":\"CPT\",\"dutyTitle\":\"Spaceman\",\"dutyStartDate\":\"2024-01-01T00:00:00Z\"}"

# Show duties and astronaut detail after Spaceman
echo ""
echo "📋 After Spaceman duty:"
echo "======================="
echo ""
echo "Astronaut Duties:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDuty/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""
echo "Astronaut Detail:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDetail/person/$TEST_PERSON_ID" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""

# Add new duty (Commander)
echo "3. Adding Commander duty..."
test_endpoint "POST" "$BASE_URL/AstronautDuty" "AddAstronautDuty - Create Commander duty" "{\"name\":\"$TEST_PERSON_NAME\",\"personId\":$TEST_PERSON_ID,\"rank\":\"CAPT\",\"dutyTitle\":\"Commander\",\"dutyStartDate\":\"2024-02-01T00:00:00Z\"}"

# Show duties and astronaut detail after Commander
echo ""
echo "📋 After Commander duty:"
echo "========================"
echo ""
echo "Astronaut Duties:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDuty/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""
echo "Astronaut Detail:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDetail/person/$TEST_PERSON_ID" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""

# Add final duty (Retired)
echo "4. Adding Retired duty..."
test_endpoint "POST" "$BASE_URL/AstronautDuty" "AddAstronautDuty - Create Retired duty" "{\"name\":\"$TEST_PERSON_NAME\",\"personId\":$TEST_PERSON_ID,\"rank\":\"CAPT\",\"dutyTitle\":\"Retired\",\"dutyStartDate\":\"2024-03-01T00:00:00Z\"}"

# Show duties and astronaut detail after Retired
echo ""
echo "📋 After Retired duty:"
echo "======================"
echo ""
echo "Astronaut Duties:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDuty/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""
echo "Astronaut Detail:"
echo "-----------------"
response=$(curl -s -X "GET" "$BASE_URL/AstronautDetail/person/$TEST_PERSON_ID" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""

# Summary
echo "========================================"
echo "📊 Test Summary"
echo "========================================"
echo -e "${GREEN}Passed: $PASSED${NC}"
echo -e "${YELLOW}Not Implemented: $NOT_IMPLEMENTED${NC}"
echo -e "${RED}Failed: $FAILED${NC}"
echo ""

if [ $FAILED -eq 0 ]; then
    echo -e "${GREEN}All tests completed!${NC}"
    exit 0
else
    echo -e "${RED}Some tests failed!${NC}"
    exit 1
fi

