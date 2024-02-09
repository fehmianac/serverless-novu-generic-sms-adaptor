# Serverless Novu Generic SMS Adaptor

This project serves as an adapter for integrating the Novu SMS provider into a serverless environment. It supports NetGSM and Twilio as SMS providers and is designed to operate on AWS using Gateway, Lambda, and AWS Parameter Store for configuration.

## Features

- Seamless integration with Novu SMS provider
- Support for NetGSM and Twilio
- Serverless architecture on AWS Gateway, Lambda, and AWS Parameter Store
- Configuration management through AWS Parameter Store

## Prerequisites

Before you begin, ensure you have the following prerequisites in place:

- AWS account
- Novu SMS provider account
- API credentials for NetGSM or Twilio
- Novu-specific configuration details (BaseURL, API Key Header, AuthUrl, Authentication Token Key, Date field, ID field)

## Configuration

1. Configure the SMS providers:

   Edit the AWS Parameter Store values for configuration details:

   - `/generic-sms-adapter/Settings`

2. Configure Novu-specific fields:

   Set the following fields on the Novu SMS provider side:

   - **BaseURL:** API-GatewayUrl /send
   - **ApiKey Header:** x-api-key
   - **AuthUrl:** API-GatewayUrl /send
   - **Authentication Token Key:** x-api-key
   - **Date Field:** date
   - **ID Field:** id

## Usage

1. Test the integration:

   Send a test SMS using the provided API or Lambda function.

2. Monitor the AWS CloudFormation stack for status and any deployment issues.

## Contributing

If you would like to contribute to this project, please follow the [contributing guidelines](CONTRIBUTING.md).

## License

This project is licensed under the [MIT License](LICENSE).

## Acknowledgements

- Mention any credits or acknowledgments to third-party libraries, resources, or contributors.
