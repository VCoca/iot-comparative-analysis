# IoT Microservices Performance Analysis

Comparative analysis of synchronous communication paradigms in IoT microservice systems using REST, gRPC, and GraphQL.

## Overview

This project explores and evaluates the performance characteristics of three widely used synchronous communication models:

* REST (HTTP/1.1 + JSON)
* gRPC (HTTP/2 + Protocol Buffers)
* GraphQL

The system simulates an environmental monitoring platform where IoT devices generate sensor measurements and backend microservices process, store, and expose data for monitoring and analytics.

The primary goal was to compare latency, throughput, resource consumption, and network efficiency across different communication approaches in realistic IoT scenarios.

---

## Architecture

The solution consists of multiple containerized services:

### Backend Services

* REST API (ASP.NET Core)
* gRPC Service (ASP.NET Core + Protocol Buffers)
* GraphQL API (Node.js + Apollo Server)

### Database

* PostgreSQL 16

### Infrastructure

* Docker Compose
* Isolated Docker Network

### Monitoring & Benchmarking

* k6 Load Testing
* Prometheus
* Grafana
* cAdvisor

---

## Test Scenarios

Three different IoT workloads were simulated.

### Scenario A – High-Frequency Data Ingestion

Large numbers of sensor readings are continuously sent to the backend.

Measured:

* Throughput (Requests/sec)
* Average Latency
* P95 Latency

### Scenario B – Selective Monitoring

Clients request only specific subsets of sensor data.

Measured:

* Query efficiency
* Data transfer optimization
* Response latency

### Scenario C – Heavy Analytics Queries

Complex aggregation and analytics operations over historical sensor data.

Measured:

* Scalability under load
* Query performance
* Resource utilization

---

## Key Findings

### gRPC

Strengths:

* Lowest latency
* Smallest payload sizes
* Excellent performance for high-frequency communication

Best suited for:

* Device-to-service communication
* High-throughput IoT environments

### REST

Strengths:

* Simplicity
* Broad compatibility
* Stable and predictable behavior

Best suited for:

* General-purpose APIs
* External integrations

### GraphQL

Strengths:

* Flexible client queries
* Reduced over-fetching
* Strong performance in read-heavy workloads

Best suited for:

* Dashboards
* Monitoring systems
* Analytics applications

---

## Performance Evaluation

The project evaluated:

* Request throughput (RPS)
* Average response latency
* P95 latency
* Network payload size
* CPU utilization
* Memory consumption

Results demonstrated that no single protocol is optimal for every scenario:

| Scenario                   | Best Performing Technology |
| -------------------------- | -------------------------- |
| High-frequency ingestion   | gRPC                       |
| Selective monitoring       | GraphQL                    |
| Complex querying           | GraphQL                    |
| Payload efficiency         | gRPC                       |
| General-purpose API design | REST                       |

---

## Learning Outcomes

Through this project I gained practical experience with:

* Microservice architecture
* ASP.NET Core development
* gRPC and Protocol Buffers
* GraphQL APIs
* Docker containerization
* PostgreSQL optimization
* Performance benchmarking
* Monitoring and observability tools
* IoT system design
