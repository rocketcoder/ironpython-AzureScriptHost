class BizRules(object):
    def TestFunction(self, message):
        return message + " testing"

    def GetMethod(self, methodName):
        method = getattr(self, methodName)
        if callable(method):
            return method
        else:
            return None

    def GetProperty(self, propertyName):
        property = getattr(self, propertyName)
        if not callable(property):
            return property
        else:
            return None