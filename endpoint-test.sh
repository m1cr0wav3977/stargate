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

# Test IDs (using 1 as default, adjust if needed)
TEST_PERSON_ID=1
TEST_PERSON_NAME="TestPerson"
TEST_ASTRONAUT_DETAIL_ID=1
TEST_ASTRONAUT_DUTY_ID=1

echo "🔄 Integration Test Flow"
echo "========================"
echo "Following entity relationships: Person → AstronautDetail → AstronautDuty"
echo ""

# ============================================
# CREATE PHASE
# ============================================
echo "📝 CREATE PHASE"
echo "---------------"

# Create Person
echo "1. Creating Person..."
test_endpoint "POST" "$BASE_URL/Person" "CreatePerson - Create a new person" "{\"Name\":\"$TEST_PERSON_NAME\"}"

# Read Person (verify creation)
echo "2. Reading Person (verify creation)..."
test_endpoint "GET" "$BASE_URL/Person" "GetPeople - Get all people"
test_endpoint "GET" "$BASE_URL/Person/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" "GetPersonByName - Get person by name"
test_endpoint "GET" "$BASE_URL/Person/$TEST_PERSON_ID" "GetPersonById - Get person by ID"

# Create AstronautDetail for Person
echo "3. Creating AstronautDetail for Person..."
test_endpoint "POST" "$BASE_URL/AstronautDetail" "CreateAstronautDetail - Create astronaut detail" "{\"personId\":$TEST_PERSON_ID,\"currentRank\":\"CPT\",\"currentDutyTitle\":\"Commander\",\"careerStartDate\":\"2024-01-01T00:00:00Z\"}"

# Read AstronautDetail (verify creation)
echo "4. Reading AstronautDetail (verify creation)..."
test_endpoint "GET" "$BASE_URL/AstronautDetail/person/$TEST_PERSON_ID" "GetAstronautDetailByPersonId - Get astronaut detail by person ID"
test_endpoint "GET" "$BASE_URL/AstronautDetail/$TEST_ASTRONAUT_DETAIL_ID" "GetAstronautDetailById - Get astronaut detail by ID"

# Create AstronautDuty for Person
echo "5. Creating AstronautDuty for Person..."
test_endpoint "POST" "$BASE_URL/AstronautDuty" "CreateAstronautDuty - Create astronaut duty" "{\"name\":\"$TEST_PERSON_NAME\",\"rank\":\"CPT\",\"dutyTitle\":\"Commander\",\"dutyStartDate\":\"2024-01-01T00:00:00Z\"}"

# Read AstronautDuty (verify creation)
echo "6. Reading AstronautDuty (verify creation)..."
test_endpoint "GET" "$BASE_URL/AstronautDuty/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" "GetAstronautDutiesByName - Get astronaut duties by name"
test_endpoint "GET" "$BASE_URL/AstronautDuty/$TEST_ASTRONAUT_DUTY_ID" "GetAstronautDutyById - Get astronaut duty by ID"

# Display Person with all related data after all creations
echo ""
echo "📋 Summary: GetPersonByName after all creations..."
echo "---------------------------------------------------"
response=$(curl -s -X "GET" "$BASE_URL/Person/name/$(echo $TEST_PERSON_NAME | sed 's/ /%20/g')" -H "Content-Type: application/json")
echo "$response" | python3 -m json.tool 2>/dev/null || echo "$response"
echo ""

# ============================================
# UPDATE PHASE
# ============================================
echo "✏️  UPDATE PHASE"
echo "----------------"

# Update Person
echo "7. Updating Person..."
test_endpoint "PUT" "$BASE_URL/Person/$TEST_PERSON_ID" "UpdatePerson - Update person by ID" "{\"personId\":$TEST_PERSON_ID,\"astronautDetailId\":0,\"currentRank\":\"CAPT\"}"

# Read Person (verify update)
echo "8. Reading Person (verify update)..."
test_endpoint "GET" "$BASE_URL/Person/$TEST_PERSON_ID" "GetPersonById - Get person by ID (after update)"

# Update AstronautDetail
echo "9. Updating AstronautDetail..."
test_endpoint "PUT" "$BASE_URL/AstronautDetail/$TEST_ASTRONAUT_DETAIL_ID" "UpdateAstronautDetail - Update astronaut detail by ID" "{\"id\":$TEST_ASTRONAUT_DETAIL_ID}"

# Read AstronautDetail (verify update)
echo "10. Reading AstronautDetail (verify update)..."
test_endpoint "GET" "$BASE_URL/AstronautDetail/$TEST_ASTRONAUT_DETAIL_ID" "GetAstronautDetailById - Get astronaut detail by ID (after update)"

# Update AstronautDuty
echo "11. Updating AstronautDuty..."
test_endpoint "PUT" "$BASE_URL/AstronautDuty/$TEST_ASTRONAUT_DUTY_ID" "UpdateAstronautDuty - Update astronaut duty by ID" "{\"id\":$TEST_ASTRONAUT_DUTY_ID}"

# Read AstronautDuty (verify update)
echo "12. Reading AstronautDuty (verify update)..."
test_endpoint "GET" "$BASE_URL/AstronautDuty/$TEST_ASTRONAUT_DUTY_ID" "GetAstronautDutyById - Get astronaut duty by ID (after update)"

echo ""

# ============================================
# DELETE PHASE (Reverse order)
# ============================================
echo "🗑️  DELETE PHASE (Reverse order)"
echo "--------------------------------"

# Delete AstronautDuty
echo "13. Deleting AstronautDuty..."
test_endpoint "DELETE" "$BASE_URL/AstronautDuty/$TEST_ASTRONAUT_DUTY_ID" "DeleteAstronautDuty - Delete astronaut duty by ID"

# Read AstronautDuty (verify deletion)
echo "14. Reading AstronautDuty (verify deletion)..."
test_endpoint "GET" "$BASE_URL/AstronautDuty/$TEST_ASTRONAUT_DUTY_ID" "GetAstronautDutyById - Get astronaut duty by ID (should fail/not found)"

# Delete AstronautDetail
echo "15. Deleting AstronautDetail..."
test_endpoint "DELETE" "$BASE_URL/AstronautDetail/$TEST_ASTRONAUT_DETAIL_ID" "DeleteAstronautDetail - Delete astronaut detail by ID"

# Read AstronautDetail (verify deletion)
echo "16. Reading AstronautDetail (verify deletion)..."
test_endpoint "GET" "$BASE_URL/AstronautDetail/$TEST_ASTRONAUT_DETAIL_ID" "GetAstronautDetailById - Get astronaut detail by ID (should fail/not found)"

# Delete Person
echo "17. Deleting Person..."
test_endpoint "DELETE" "$BASE_URL/Person/$TEST_PERSON_ID" "DeletePerson - Delete person by ID"

# Read Person (verify deletion)
echo "18. Reading Person (verify deletion)..."
test_endpoint "GET" "$BASE_URL/Person/$TEST_PERSON_ID" "GetPersonById - Get person by ID (should fail/not found)"

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

