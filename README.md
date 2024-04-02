# Stock Metrics API

This repository contains code for a REST API that provides stock metrics, including daily return and alpha values.

## Description

The Stock Metrics API allows users to retrieve historical stock data and calculate metrics such as daily return and alpha values. It leverages the IEX Cloud API for retrieving historical stock prices.

### Features

- **Get Return**: Retrieve daily return for a specified stock within a given date range.
- **Get Alpha**: Calculate the alpha value of a stock compared to a benchmark within a specified date range.

## Setup

To set up the project locally, follow these steps:

1. Clone the repository:
   ```bash
   https://github.com/dagmawihm/StockMetricsAPI.Net.git
   
3. Set up environment variables:
- **.env**: Create a .env file in the root directory.
- **IEX Cloud API token**: Add your IEX Cloud API token to the .env file. IEX_TOKEN=your_iex_cloud_api_token


## Usage

1. Starting the Server
-The server will start listening on port 5246 by default.


## Endpoints

### 1. Get Return
##### Endpoint: /api/return
##### Method: GET
##### Parameters:
- **symbol**: Stock ticker symbol (required)
- **from**: Start date in YYYY-MM-DD format (optional)
- **to**: End date in YYYY-MM-DD format (optional)
##### Description: Retrieves historical return for the specified stock within the specified date range.
##### Example:
      GET /api/return?symbol=AAPL&from=2024-01-01&to=2024-01-28


### 2. Get Alpha
##### Endpoint: /api/alpha
##### Method: GET
##### Parameters:
- **symbol**: Stock ticker symbol (required)
- **benchmark**: Benchmark ticker symbol (required)
- **from**: Start date in YYYY-MM-DD format (optional)
- **to**: End date in YYYY-MM-DD format (optional)
##### Description: Calculates the alpha (excess return) of the specified stock compared to the benchmark within the specified date range.
##### Example:
      GET /api/alpha?symbol=AAPL&benchmark=DJIA&from=2024-01-01&to=2024-01-28


## Error Handling
- The API handles various error scenarios, including invalid input parameters and internal server errors. Detailed error messages are provided in JSON format.

## Configuration
- Configure the API behavior by modifying the code or environment variables.
- Default settings include port numbers, API token, and validation rules.

## Contributing
- Contributions are welcome! If you have any ideas, enhancements, or bug fixes, feel free to open an issue or submit a pull request.
