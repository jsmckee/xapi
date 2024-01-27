﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;

using Korelight.XAPI.Core;

namespace Korelight.XAPI
{
    /// <summary>
    /// SWIFT (iOS) API class, generates .swift file for the target service type.
    /// </summary>
    public class SwiftAPI<ControllerType> : XCore<ControllerType>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SwiftAPI() : base() { }

        /// <summary>
        /// Get a runtime computed SWIFT API for a given Web API service Type.
        /// </summary>
        /// <param name="servicetype"></param>
        /// <returns></returns>
        public override Stream GetAPI(String controller)
        {
            StringBuilder api = new StringBuilder();

            api.Append("// SWIFT API Generated by the Korelight XAPI Service.");
            api.Append(Environment.NewLine);
            api.Append("// http://www.Korelight.com ");
            api.Append(Environment.NewLine);
            api.Append("// Jeremy McKee - 7/14/2017");
            api.Append(Environment.NewLine);
            api.Append("// Timestamp: ");
            api.Append(DateTime.UtcNow.ToString());
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);
            api.Append("import Foundation");
            api.Append(Environment.NewLine);
            api.Append("class ");
            api.Append(typeof(ControllerType).Name);
            api.Append("API { ");
            api.Append(Environment.NewLine);
            api.Append("private func dataTask(request: NSMutableURLRequest, method: String, completion: @escaping ( Bool,  AnyObject?) -> ()) {");
            api.Append(Environment.NewLine);
            api.Append("request.httpMethod = method");
            api.Append(Environment.NewLine);
            api.Append("let session = URLSession(configuration: URLSessionConfiguration.default)");
            api.Append(Environment.NewLine);

            api.Append("session.dataTask(with: request as URLRequest) { (data, response, error) -> Void in");
            api.Append(Environment.NewLine);
            api.Append("if let data = data {");
            api.Append(Environment.NewLine);
            api.Append("let json = try? JSONSerialization.jsonObject(with: data, options: [])");
            api.Append(Environment.NewLine);
            api.Append("if let response = response as? HTTPURLResponse, 200...299 ~= response.statusCode {");
            api.Append(Environment.NewLine);
            api.Append("completion(true, json as AnyObject)");
            api.Append(Environment.NewLine);
            api.Append("} else {");
            api.Append(Environment.NewLine);
            api.Append("completion(false, json as AnyObject)");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append("}.resume()");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);

            api.Append("private func clientURLRequest(path: String, params: String) -> NSMutableURLRequest {");
            api.Append(Environment.NewLine);
            api.Append("let request = NSMutableURLRequest(url: NSURL(string: path)! as URL)");
            api.Append(Environment.NewLine);
            api.Append("request.setValue(\"application/json\", forHTTPHeaderField: \"Content-Type\")");
            api.Append(Environment.NewLine);
            api.Append("request.httpBody = params.data(using: String.Encoding.utf8)");
            api.Append(Environment.NewLine);

            api.Append("return request");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);
            api.Append("private func callpost(request: NSMutableURLRequest, completion: @escaping ( Bool, AnyObject?) -> ()) {");
            api.Append(Environment.NewLine);
            api.Append("dataTask(request: request, method: \"POST\", completion: completion)");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);

            api.Append("private func callput(request: NSMutableURLRequest, completion: @escaping ( Bool, AnyObject?) -> ()) {");
            api.Append(Environment.NewLine);
            api.Append("dataTask(request: request, method: \"PUT\", completion: completion)");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);

            api.Append("private func callget(request: NSMutableURLRequest, completion: @escaping (Bool, AnyObject?) -> ()) {");
            api.Append(Environment.NewLine);
            api.Append("dataTask(request: request, method: \"GET\", completion: completion)");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);
            api.Append("private func calldelete(request: NSMutableURLRequest, completion: @escaping (Bool, AnyObject?) -> ()) {");
            api.Append(Environment.NewLine);
            api.Append("dataTask(request: request, method: \"DELETE\", completion: completion)");
            api.Append(Environment.NewLine);
            api.Append("}");
            api.Append(Environment.NewLine);
            api.Append(Environment.NewLine);




            foreach (MethodDocumentation doc in GetTypeDocumentation())
            {
                StringBuilder data = new StringBuilder();
                api.Append(Environment.NewLine);
                api.Append("public func ");

                api.Append(doc.MethodName);
                api.Append("(completion: @escaping (Bool, AnyObject?) ->(), host: String");

                if (doc.MethodParameters.Count > 0)
                {
                    api.Append(",");
                }
                else
                {
                    api.Append("){");
                }

                foreach (var p in doc.MethodParameters)
                {
                    api.Append("params: AnyObject?");
                    data.Append(p.ParameterName);

                    api.Append("){");
                    break;
                }

                var endpoint = doc.MethodName;

                RouteAttribute route = null;
                foreach (Attribute a in doc.MethodAttributes)
                {
                    if (a as RouteAttribute != null)
                    {
                        route = a as RouteAttribute;
                    }
                }

                if (route != null)
                {
                    endpoint = route.Template;
                }


                api.Append(Environment.NewLine);
                //api.Append(Environment.NewLine);
                if (doc.MethodParameters.Count > 0)
                {
                    switch (doc.HttpActionType)
                    {
                        case "GET":
                            api.Append("self.callget(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (params as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "PUT":
                            api.Append("self.callput(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (params as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "POST":
                            api.Append("self.callpost(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (params as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "DELETE":
                            api.Append("self.calldelete(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (params as AnyObject) as! String)) {(success, object) -> () in ");
                            break;
                    }

                }
                else
                {
                    switch (doc.HttpActionType)
                    {
                        case "GET":
                            api.Append("self.callget(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (\"\" as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "PUT":
                            api.Append("self.callput(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (\"\" as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "POST":
                            api.Append("self.callpost(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (\"\" as AnyObject) as! String)) {(success, object) -> () in ");
                            break;

                        case "DELETE":
                            api.Append("self.calldelete(request: clientURLRequest(path: host + \"" + controller + "/" + endpoint + "\", params: (\"\" as AnyObject) as! String)) {(success, object) -> () in ");
                            break;
                    }

                }

                api.Append(Environment.NewLine);
                api.Append("DispatchQueue.main.async(execute: { () -> Void in");
                api.Append(Environment.NewLine);
                api.Append("if success {");
                api.Append(Environment.NewLine);
                api.Append("completion(true, object)");
                api.Append(Environment.NewLine);
                api.Append("} else {");
                api.Append(Environment.NewLine);
                api.Append(" var message = \"There was an Error\"");
                api.Append(Environment.NewLine);
                api.Append("if let object = object, let passedMessage = object[\"message\"] as? String {");
                api.Append(Environment.NewLine);
                api.Append("message = passedMessage");
                api.Append(Environment.NewLine);
                api.Append("}");
                api.Append(Environment.NewLine);
                api.Append("completion(false, message as AnyObject)");
                api.Append(Environment.NewLine);
                api.Append("}");
                api.Append(Environment.NewLine);
                api.Append("})");
                api.Append(Environment.NewLine);
                api.Append("}");
                api.Append(Environment.NewLine);
                api.Append("}");
                api.Append(Environment.NewLine);
            }

            api.Append("}");
            MemoryStream result = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(api.ToString()));
            return result;
        }


        public override List<Tuple<FileInfo, MemoryStream>> GetCoreServiceClients()
        {
            return new List<Tuple<FileInfo, MemoryStream>>();
        }

        public override List<Tuple<FileInfo, MemoryStream>> GetInterfaces(Type[] targettypes)
        {
            return new List<Tuple<FileInfo, MemoryStream>>();
        }
    }
}