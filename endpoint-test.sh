#!/bin/bash

# Test script for all Query endpoints in the Stargate API
# This script tests all GET endpoints that use Queries

BASE_URL="http://localhost:5204"
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "🧪 Testing Stargate API Query Endpoints"
echo "========================================"
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
    
    echo -n "Testing: $description ... "
    
    response=$(curl -s -w "\n%{http_code}" -X "$method" "$url" 2>&1)
    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')
    
    if [ "$http_code" -eq 200 ]; then
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

# Person Controller Queries
echo "📋 Person Controller Queries"
echo "----------------------------"
test_endpoint "GET" "$BASE_URL/Person" "GetPeople - Get all people"
test_endpoint "GET" "$BASE_URL/Person/name/John%20Doe" "GetPersonByName - Get person by name"
test_endpoint "GET" "$BASE_URL/Person/1" "GetPersonById - Get person by ID"

# AstronautDuty Controller Queries
echo "🚀 AstronautDuty Controller Queries"
echo "------------------------------------"
test_endpoint "GET" "$BASE_URL/AstronautDuty/name/John%20Doe" "GetAstronautDutiesByName - Get astronaut duties by name"
test_endpoint "GET" "$BASE_URL/AstronautDuty/1" "GetAstronautDutyById - Get astronaut duty by ID"

# AstronautDetail Controller Queries
echo "👨‍🚀 AstronautDetail Controller Queries"
echo "--------------------------------------"
test_endpoint "GET" "$BASE_URL/AstronautDetail/person/1" "GetAstronautDetailByPersonId - Get astronaut detail by person ID"
test_endpoint "GET" "$BASE_URL/AstronautDetail/1" "GetAstronautDetailById - Get astronaut detail by ID"

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

