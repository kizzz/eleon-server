# HealthChecks Migration Deployment Plan

## Overview

This document provides a comprehensive deployment plan for the HealthChecks V2 migration, covering staging and production deployments.

## Pre-Deployment Checklist

### Code Verification
- [x] All code migrated to V2 architecture
- [x] All projects build successfully
- [ ] All unit tests pass (some failures need fixing)
- [ ] Integration tests pass (requires Docker)
- [x] Configuration files updated with V2 options

### Configuration Verification
- [x] Development configs updated (`appsettings.app.json`)
- [x] Production configs updated (`appsettings.Release.json`)
- [ ] QA configs reviewed (`appsettings.QA.json`)
- [ ] Debug configs reviewed (`appsettings.Debug.json`)
- [ ] All production configs have `RestartEnabled: false`
- [ ] All production configs have `EnableDiagnostics: false`

### Documentation
- [x] Migration guide created
- [x] Architecture documentation created
- [x] Security documentation created
- [x] Configuration update guide created
- [x] Testing guide created

## Phase 1: Staging Deployment

### 1.1 Pre-Deployment

**Timeline:** Day 1

**Tasks:**
1. **Final Code Review**
   - Review all migrated files
   - Verify no breaking changes
   - Check rollback plan

2. **Configuration Review**
   - Review staging configuration
   - Verify all V2 options present
   - Ensure `RestartEnabled: false`
   - Ensure `EnableDiagnostics: false`

3. **Build Verification**
   - Build all projects
   - Verify no compilation errors
   - Check for warnings

4. **Test Execution**
   - Run unit tests (fix failures first)
   - Run integration tests (if Docker available)
   - Document test results

5. **Team Notification**
   - Notify team of deployment
   - Share migration guide
   - Schedule deployment window

### 1.2 Deployment

**Timeline:** Day 2

**Deployment Window:** Low-traffic period (recommended: early morning)

**Steps:**
1. **Backup Current Deployment**
   - Backup current application binaries
   - Backup configuration files
   - Document current version

2. **Deploy New Version**
   - Deploy updated binaries
   - Deploy updated configuration
   - Verify deployment successful

3. **Immediate Verification** (First 5 minutes)
   - [ ] Application starts without errors
   - [ ] Health endpoints respond:
     - `GET /health/live` - 200 OK
     - `GET /health/ready` - 200 OK or 503
     - `GET /health/diag` - Requires auth
   - [ ] No exceptions in logs
   - [ ] Health checks execute successfully

4. **Initial Health Checks** (First 15 minutes)
   - [ ] All registered checks run
   - [ ] SQL Server checks work (no DB creation)
   - [ ] HTTP checks validate endpoints
   - [ ] Configuration checks validate settings
   - [ ] Snapshots created successfully

5. **Monitoring** (First 30 minutes)
   - [ ] Monitor application logs
   - [ ] Monitor health check execution
   - [ ] Monitor error rates
   - [ ] Monitor response times
   - [ ] Verify publishing (if enabled)

### 1.3 Post-Deployment Verification

**Timeline:** Day 2-3

**Tasks:**
1. **Functional Testing**
   - Test all health endpoints
   - Verify authentication on privileged endpoints
   - Test manual health check trigger
   - Verify UI page (if enabled)

2. **Performance Testing**
   - Measure response times
   - Test concurrent requests
   - Verify single-run enforcement
   - Check resource usage

3. **Security Testing**
   - Verify endpoint authentication
   - Test output scrubbing
   - Verify SQL Server safety (no DB creation)
   - Test with read-only SQL permissions

4. **Integration Testing**
   - Test with dependent services
   - Verify publishing works
   - Test error scenarios
   - Verify timeout handling

### 1.4 Staging Sign-Off

**Criteria:**
- [ ] All health endpoints work correctly
- [ ] All checks execute successfully
- [ ] No errors in logs
- [ ] Performance acceptable
- [ ] Security verified
- [ ] Team comfortable with changes

**Timeline:** 2-3 days in staging

## Phase 2: Production Deployment

### 2.1 Pre-Deployment

**Timeline:** Day 4-5

**Prerequisites:**
- [ ] Staging deployment successful
- [ ] All staging tests passed
- [ ] No critical issues found
- [ ] Team sign-off obtained

**Tasks:**
1. **Final Configuration Review**
   - Review production configuration
   - Verify `RestartEnabled: false`
   - Verify `EnableDiagnostics: false`
   - Verify connection strings
   - Verify authentication settings

2. **Rollback Plan**
   - Document rollback procedure
   - Test rollback in staging
   - Prepare rollback package
   - Have rollback ready

3. **Monitoring Setup**
   - Configure health check alerts
   - Set up error rate monitoring
   - Configure response time alerts
   - Set up publishing monitoring

4. **Team Preparation**
   - Notify team of deployment
   - Schedule deployment window
   - Assign on-call personnel
   - Prepare runbook

### 2.2 Deployment

**Timeline:** Day 6

**Deployment Window:** Maintenance window (low-traffic period)

**Steps:**
1. **Pre-Deployment Checks**
   - Verify staging still healthy
   - Check current production health
   - Verify backup available
   - Confirm team ready

2. **Deploy to Production**
   - Deploy during maintenance window
   - Deploy updated binaries
   - Deploy updated configuration
   - Verify deployment successful

3. **Immediate Verification** (First 5 minutes)
   - [ ] Application starts without errors
   - [ ] Health endpoints respond correctly
   - [ ] No exceptions in logs
   - [ ] Health checks execute

4. **Initial Monitoring** (First 15 minutes)
   - [ ] Monitor application logs
   - [ ] Monitor health check execution
   - [ ] Monitor error rates
   - [ ] Monitor response times
   - [ ] Verify no database creation

5. **Extended Monitoring** (First hour)
   - [ ] Monitor continuously
   - [ ] Check for any issues
   - [ ] Verify publishing works
   - [ ] Monitor system resources

### 2.3 Post-Deployment

**Timeline:** Day 6-13 (First week)

**First 24 Hours:**
- [ ] Monitor health check execution
- [ ] Monitor error rates
- [ ] Monitor response times
- [ ] Check publishing success rate
- [ ] Verify no database creation
- [ ] Monitor system resources
- [ ] Review logs daily

**First Week:**
- [ ] Daily health check review
- [ ] Monitor for regressions
- [ ] Collect performance metrics
- [ ] Document any issues
- [ ] Plan legacy code removal

## Rollback Plan

### Rollback Procedure

If critical issues occur:

1. **Immediate Rollback**
   - Revert to previous deployment
   - Restore previous configuration
   - Verify application starts
   - Monitor for stability

2. **Configuration Rollback**
   - Revert to old `AddCommonHealthChecks` registration
   - Comment out V2 registration
   - Uncomment old registration
   - Restart application

3. **Verification**
   - Verify application works
   - Verify health endpoints work
   - Monitor for issues
   - Document rollback reason

### Rollback Triggers

Rollback if:
- Application fails to start
- Health endpoints don't respond
- Critical errors in logs
- Performance degradation > 20%
- Database creation detected
- Security issues found

## Monitoring and Alerts

### Key Metrics to Monitor

1. **Health Check Execution**
   - Execution frequency
   - Success rate
   - Execution duration
   - Timeout rate

2. **Endpoint Performance**
   - Response times
   - Error rates
   - Availability

3. **System Resources**
   - CPU usage
   - Memory usage
   - Database connections
   - Network usage

4. **Publishing** (if enabled)
   - Publishing success rate
   - Publishing frequency
   - Publishing errors

### Alerts to Configure

1. **Critical Alerts**
   - Health check failures > 50%
   - Application startup failures
   - Database creation detected
   - Security violations

2. **Warning Alerts**
   - Health check failures > 10%
   - Response time > threshold
   - Publishing failures
   - High error rates

## Success Criteria

### Staging Success
- [ ] All health endpoints work
- [ ] All checks execute successfully
- [ ] No errors in logs
- [ ] Performance acceptable
- [ ] Security verified
- [ ] Team sign-off

### Production Success
- [ ] Staging tests passed
- [ ] Production deployment successful
- [ ] No critical issues
- [ ] Monitoring confirms health
- [ ] Team comfortable
- [ ] Ready for legacy cleanup

## Timeline

- **Day 1:** Pre-deployment preparation
- **Day 2:** Staging deployment
- **Day 2-3:** Staging verification
- **Day 4-5:** Production preparation
- **Day 6:** Production deployment
- **Day 6-13:** Production monitoring (first week)
- **Week 2-4:** Extended monitoring
- **Week 5+:** Legacy code removal (after verification)

## Risk Mitigation

### Identified Risks

1. **Test Failures**
   - **Risk:** Some unit tests failing
   - **Mitigation:** Fix test issues before deployment
   - **Impact:** Low (tests can be fixed post-deployment)

2. **Configuration Issues**
   - **Risk:** Missing or incorrect configuration
   - **Mitigation:** Comprehensive configuration review
   - **Impact:** Medium (can cause runtime issues)

3. **Performance Degradation**
   - **Risk:** New architecture might be slower
   - **Mitigation:** Performance testing in staging
   - **Impact:** Low (architecture is more efficient)

4. **SQL Server Safety**
   - **Risk:** Database creation or mutations
   - **Mitigation:** Comprehensive safety tests
   - **Impact:** Critical (but mitigated by safety guarantees)

### Mitigation Strategies

- Comprehensive testing
- Staged deployment
- Monitoring and alerting
- Rollback plan ready
- Team training
- Documentation complete

## Communication Plan

### Pre-Deployment
- Notify team 1 week before
- Share migration guide
- Schedule deployment window
- Assign responsibilities

### During Deployment
- Real-time updates
- Status reports
- Issue notifications

### Post-Deployment
- Daily status updates (first week)
- Weekly reviews
- Issue documentation
- Success celebration

## Next Steps After Deployment

1. **Monitor** for 2-4 weeks
2. **Collect** metrics and feedback
3. **Document** any issues
4. **Plan** legacy code removal
5. **Update** runbooks
6. **Train** team on new architecture
