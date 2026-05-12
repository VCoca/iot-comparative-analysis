import { ApolloServer } from '@apollo/server';
import { startStandaloneServer } from '@apollo/server/standalone';
import pkg from 'pg';
const { Pool } = pkg;

// Konekcija ka bazi
const pool = new Pool({
  user: 'iotadmin',
  host: 'localhost',
  database: 'iot_db',
  password: 'admin',
  port: 5432,
});

const typeDefs = `#graphql
  type SensorData {
    id: ID!
    device_id: String
    recorded_at: String
    co_gt: Float
    temperature: Float
    relative_humidity: Float
  }

  type Aggregation {
    avg_temperature: Float
    max_co: Float
    min_humidity: Float
  }

  type Query {
    latestData(device_id: String!): SensorData
    
    aggregateData(device_id: String!, start: String!, end: String!): Aggregation
  }

  type Mutation {
    addSensorData(
      device_id: String!, 
      recorded_at: String!, 
      co_gt: Float, 
      temperature: Float, 
      relative_humidity: Float
    ): Boolean
  }
`;

const resolvers = {
  Query: {
    latestData: async (_, { device_id }) => {
      const res = await pool.query(
        'SELECT id, device_id, recorded_at, co_gt, temperature, relative_humidity FROM sensor_data WHERE device_id = $1 ORDER BY recorded_at DESC LIMIT 1',
        [device_id]
      );
      return res.rows[0];
    },
    aggregateData: async (_, { device_id, start, end }) => {
      const res = await pool.query(
        `SELECT 
          AVG(temperature) as avg_temperature, 
          MAX(co_gt) as max_co, 
          MIN(relative_humidity) as min_humidity
         FROM sensor_data 
         WHERE device_id = $1 AND recorded_at >= $2 AND recorded_at <= $3`,
        [device_id, start, end]
      );
      return res.rows[0];
    }
  },
  Mutation: {
    addSensorData: async (_, args) => {
      const res = await pool.query(
        'INSERT INTO sensor_data (device_id, recorded_at, co_gt, temperature, relative_humidity) VALUES ($1, $2, $3, $4, $5)',
        [args.device_id, args.recorded_at, args.co_gt, args.temperature, args.relative_humidity]
      );
      return res.rowCount > 0;
    }
  }
};

const server = new ApolloServer({ typeDefs, resolvers });

const { url } = await startStandaloneServer(server, { listen: { port: 4000 } });

console.log(`GraphQL server je spreman na adresi: ${url}`);